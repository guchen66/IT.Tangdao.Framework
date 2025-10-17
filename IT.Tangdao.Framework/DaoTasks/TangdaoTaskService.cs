using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Parameters.Infrastructure;

namespace IT.Tangdao.Framework.DaoTasks
{
    public class TangdaoTaskService : ITangdaoTaskService, IDisposable
    {
        private readonly ManualResetEventSlim _manual = new ManualResetEventSlim();
        private CancellationTokenSource _cts;
        private volatile bool _isPaused;
        private object lockObject = new object();

        public async Task StartAsync(IProgress<IAddTaskItem> progress)
        {
            lock (lockObject)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                _manual.Set();

                _isPaused = false;
            }

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    // 检查暂停状态

                    _cts.Token.ThrowIfCancellationRequested();

                    Random random = new Random();
                    int randomNumber = random.Next(0, 11); // 0-10（包含0，不包含11）
                    bool result = randomNumber > 5 ? true : false;
                    progress.Report(new AddTaskItem()
                    {
                        NewItem = "未完成",
                    });

                    await Task.Delay(1000, _cts.Token);
                    if (_isPaused)
                    {
                        await Task.Run(() => { _manual.Wait(_cts.Token); });
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("任务已取消");
            }
        }

        public void Pause()
        {
            lock (lockObject)
            {
                _isPaused = true;
                _manual.Reset();
            }
        }

        public void Resume()
        {
            lock (lockObject)
            {
                _isPaused = false;
                _manual.Set();
            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                _cts?.Cancel();
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _manual?.Dispose();
        }
    }
}