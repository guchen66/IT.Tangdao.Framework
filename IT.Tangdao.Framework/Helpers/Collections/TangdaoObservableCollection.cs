using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 增强型ObservableCollection，支持批量操作和通知抑制
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    public class TangdaoObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// 通知抑制计数器，用于支持嵌套抑制
        /// </summary>
        private int _suppressCount = 0;

        /// <summary>
        /// 暂停集合通知
        /// </summary>
        /// <returns>可释放对象，用于恢复通知</returns>
        public IDisposable SuspendNotifications()
        {
            return new NotifyToken(this);
        }

        /// <summary>
        /// 通知令牌，用于管理通知的暂停和恢复
        /// </summary>
        public struct NotifyToken : IDisposable
        {
            private readonly TangdaoObservableCollection<T> _collection;
            private bool _disposed;

            /// <summary>
            /// 初始化通知令牌
            /// </summary>
            /// <param name="collection">目标集合</param>
            public NotifyToken(TangdaoObservableCollection<T> collection)
            {
                _collection = collection;
                _disposed = false;
                collection._suppressCount++;
            }

            /// <summary>
            /// 释放令牌，恢复通知
            /// </summary>
            public void Dispose()
            {
                if (!_disposed)
                {
                    _collection._suppressCount--;
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="items">要添加的元素集合</param>
        /// <exception cref="ArgumentNullException">当items为null时抛出</exception>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var list = items.ToList();
            if (list.Count == 0) return;

            using (SuspendNotifications())
            {
                foreach (var item in list)
                {
                    Items.Add(item);
                }
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, list));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <summary>
        /// 批量移除元素
        /// </summary>
        /// <param name="items">要移除的元素集合</param>
        /// <exception cref="ArgumentNullException">当items为null时抛出</exception>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var list = items.ToList();
            if (list.Count == 0) return;

            var indices = list.Select(item => Items.IndexOf(item))
                              .Where(index => index >= 0)
                              .OrderByDescending(i => i)
                              .ToList();

            using (SuspendNotifications())
            {
                foreach (var index in indices)
                {
                    Items.RemoveAt(index);
                }
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, list));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <summary>
        /// 刷新集合内容
        /// </summary>
        /// <param name="newItems">新的元素集合</param>
        /// <exception cref="ArgumentNullException">当newItems为null时抛出</exception>
        public void Refresh(IEnumerable<T> newItems)
        {
            if (newItems == null) throw new ArgumentNullException(nameof(newItems));

            var list = newItems.ToList();

            using (SuspendNotifications())
            {
                // 清空现有内容
                Items.Clear();
                // 添加新内容
                foreach (var item in list)
                {
                    Items.Add(item);
                }
            }

            // 通知集合重置
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <summary>
        /// 开始批量操作
        /// </summary>
        /// <returns>可释放对象，用于结束批量操作</returns>
        public IDisposable BeginBulkOperation()
        {
            return SuspendNotifications();
        }

        /// <summary>
        /// 重写集合变更通知方法
        /// </summary>
        /// <param name="e">集合变更事件参数</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suppressCount == 0)
            {
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// 重写属性变更通知方法
        /// </summary>
        /// <param name="e">属性变更事件参数</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_suppressCount == 0)
            {
                base.OnPropertyChanged(e);
            }
        }
    }
}