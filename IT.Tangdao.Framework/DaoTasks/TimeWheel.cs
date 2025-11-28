using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    /// <summary>
    /// 时间轮调度器
    /// </summary>
    /// <typeparam name="T">任务数据类型</typeparam>
    /// <remarks>
    /// 时间轮是一种高效的定时器实现，用于处理大量定时任务。
    /// 本实现为60秒时间轮，每秒转动一格，每个格子对应一个任务队列。
    /// 当时间轮转动到某个格子时，执行该格子中的所有任务。
    ///
    /// 设计思路：
    /// 1. 使用数组（字典）表示时间轮的各个槽位
    /// 2. 每个槽位对应一个并发队列，存储该时间点需要执行的任务
    /// 3. 使用定时器每秒触发一次，转动时间轮
    /// 4. 任务添加时根据延迟时间计算目标槽位，放入对应队列
    /// 5. 时间轮转动时，执行当前槽位的所有任务
    ///
    /// 优点：
    /// - 高效处理大量定时任务，时间复杂度为O(1)
    /// - 内存占用稳定，不会随任务数量线性增长
    /// - 支持动态添加和删除任务
    /// - 支持异步任务执行
    /// </remarks>
    public class TimeWheel<T> : IDisposable
    {
        /// <summary>
        /// 当前时间轮指针位置（秒槽位），范围0-59
        /// </summary>
        private int _secondSlot = 0;

        /// <summary>
        /// 定时器，每秒触发一次，用于驱动时间轮转动
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 时间轮槽位字典，key为槽位索引（0-59），value为该槽位的任务队列
        /// </summary>
        private readonly Dictionary<int, ConcurrentQueue<WheelTask<T>>> _secondTaskQueue;

        /// <summary>
        /// 线程安全锁，用于保护共享资源
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 时间轮运行状态标记
        /// </summary>
        private volatile bool _isRunning = false;

        /// <summary>
        /// 资源释放标记，防止重复释放
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 初始化TimeWheel类的新实例
        /// </summary>
        /// <remarks>
        /// 创建一个60秒的时间轮，初始化所有槽位和定时器
        /// </remarks>
        public TimeWheel()
        {
            // 初始化60个槽位，每个槽位对应一个并发队列
            _secondTaskQueue = new Dictionary<int, ConcurrentQueue<WheelTask<T>>>();
            for (int i = 0; i < 60; i++)
            {
                _secondTaskQueue.Add(i, new ConcurrentQueue<WheelTask<T>>());
            }

            // 初始化定时器，初始状态为停止，每秒执行一次
            _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 启动时间轮
        /// </summary>
        /// <remarks>
        /// 启动定时器，开始驱动时间轮转动
        /// 线程安全，多次调用不会重复启动
        /// </remarks>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                // 立即开始，每秒执行一次
                _timer.Change(0, 1000);
            }
        }

        /// <summary>
        /// 停止时间轮
        /// </summary>
        /// <remarks>
        /// 停止定时器，时间轮不再转动
        /// 线程安全，多次调用不会重复停止
        /// </remarks>
        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                    return;

                _isRunning = false;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 异步添加定时任务
        /// </summary>
        /// <param name="delaySeconds">延迟秒数，任务将在指定秒数后执行</param>
        /// <param name="data">任务携带的数据</param>
        /// <param name="handler">任务处理函数（异步）</param>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="ArgumentNullException">当handler为null时抛出</exception>
        /// <exception cref="ObjectDisposedException">当时间轮已释放时抛出</exception>
        /// <remarks>
        /// 添加一个定时任务到时间轮中，任务将在指定延迟后执行
        /// 如果延迟为0或目标槽位就是当前槽位，任务将立即执行
        /// </remarks>
        public async Task AddTaskAsync(int delaySeconds, T data, Func<T, Task> handler)
        {
            // 参数验证
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // 确保延迟秒数为非负数
            if (delaySeconds < 0)
                delaySeconds = 0;

            // 检查时间轮是否已释放
            lock (_lock)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(TimeWheel<T>));
            }

            // 计算任务应该放入的目标槽位
            int targetSlot = (_secondSlot + delaySeconds) % 60;

            // 如果延迟为0或目标槽位就是当前槽位，立即执行任务
            if (delaySeconds == 0 || targetSlot == _secondSlot)
            {
                await handler(data);
                return;
            }

            // 将任务放入目标槽位的队列中
            _secondTaskQueue[targetSlot].Enqueue(new WheelTask<T>(data, handler));
        }

        /// <summary>
        /// 定时器回调函数，每秒执行一次
        /// </summary>
        /// <param name="state">定时器状态参数（未使用）</param>
        private async void Callback(object state)
        {
            // 如果时间轮已停止，直接返回
            if (!_isRunning)
                return;

            try
            {
                // 执行当前槽位的所有任务
                await ExecuteTasksForCurrentSlot();

                // 更新时间轮指针位置，指向下一个槽位
                lock (_lock)
                {
                    _secondSlot = (_secondSlot + 1) % 60;
                }
            }
            catch (Exception ex)
            {
                // 记录异常，但不影响定时器继续运行
                // 这样即使某个任务执行失败，也不会导致整个时间轮停止
                Console.WriteLine($"TimeWheel callback error: {ex.Message}");
            }
        }

        /// <summary>
        /// 执行当前槽位的所有任务
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        private async Task ExecuteTasksForCurrentSlot()
        {
            // 获取当前槽位，使用锁保护共享资源
            int currentSlot;
            lock (_lock)
            {
                currentSlot = _secondSlot;
            }

            // 获取当前槽位的任务队列
            var queue = _secondTaskQueue[currentSlot];

            // 遍历执行当前槽位的所有任务
            while (queue.TryDequeue(out var task))
            {
                try
                {
                    // 执行任务
                    await task.Handler(task.Data);
                }
                catch (Exception ex)
                {
                    // 记录异常，但不影响其他任务执行
                    // 这样即使某个任务执行失败，也不会影响同槽位的其他任务
                    Console.WriteLine($"TimeWheel task error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 释放时间轮资源
        /// </summary>
        /// <remarks>
        /// 释放定时器资源，停止时间轮
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放时间轮资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        /// <remarks>
        /// 实现IDisposable接口的标准释放模式
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 停止时间轮
                    Stop();
                    // 释放定时器资源
                    _timer.Dispose();
                }

                _disposed = true;
            }
        }
    }
}