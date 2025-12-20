using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Notices
{
    /// <summary>
    /// 泛型通知发布者实现，支持发布特定类型的通知给订阅者
    /// </summary>
    /// <typeparam name="TNotification">通知类型</typeparam>
    public class TangdaoPublisher<TNotification> : ITangdaoPublisher<TNotification>
    {
        /// <summary>
        /// 订阅者列表
        /// </summary>
        private readonly List<IObserver<TNotification>> _observers = new List<IObserver<TNotification>>();

        /// <summary>
        /// 用于线程同步的锁
        /// </summary>
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// 是否已完成
        /// </summary>
        private bool _isCompleted = false;

        /// <summary>
        /// 订阅通知
        /// </summary>
        /// <param name="observer">通知观察者</param>
        /// <returns>用于取消订阅的IDisposable对象</returns>
        public IDisposable Subscribe(IObserver<TNotification> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer), "观察者不能为空");
            }

            try
            {
                _rwLock.EnterWriteLock();

                if (_isCompleted)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                _observers.Add(observer);
                return new Unsubscriber(_observers, observer, _rwLock);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 发布通知给所有订阅者
        /// </summary>
        /// <param name="notification">要发布的通知</param>
        public void Publish(TNotification notification)
        {
            IObserver<TNotification>[] observersCopy;

            try
            {
                _rwLock.EnterReadLock();
                observersCopy = _observers.ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            foreach (var observer in observersCopy)
            {
                try
                {
                    observer.OnNext(notification);
                }
                catch (Exception ex)
                {
                    // 捕获并处理单个观察者的异常，确保其他观察者仍能收到通知
                    observer.OnError(ex);
                }
            }
        }

        /// <summary>
        /// 完成所有订阅，不再接受新的通知，并清理资源
        /// </summary>
        public void CompleteAll()
        {
            IObserver<TNotification>[] observersCopy;

            try
            {
                _rwLock.EnterWriteLock();

                if (_isCompleted)
                {
                    return;
                }

                _isCompleted = true;
                observersCopy = _observers.ToArray();
                _observers.Clear();
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }

            foreach (var observer in observersCopy)
            {
                try
                {
                    observer.OnCompleted();
                }
                catch
                {
                    // 忽略完成通知时的异常
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CompleteAll();
            _rwLock.Dispose();
        }

        /// <summary>
        /// 用于取消订阅的内部类
        /// </summary>
        private sealed class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<TNotification>> _observers;
            private readonly IObserver<TNotification> _observer;
            private readonly ReaderWriterLockSlim _rwLock;
            private bool _isDisposed = false;

            public Unsubscriber(List<IObserver<TNotification>> observers, IObserver<TNotification> observer, ReaderWriterLockSlim rwLock)
            {
                _observers = observers;
                _observer = observer;
                _rwLock = rwLock;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                try
                {
                    _rwLock.EnterWriteLock();
                    if (_observers.Contains(_observer))
                    {
                        _observers.Remove(_observer);
                    }
                    _isDisposed = true;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
        }
    }
}