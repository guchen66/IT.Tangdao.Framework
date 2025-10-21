using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 增强版 CountdownEvent：支持一次减 N、动态加 N、父子级联
    /// </summary>
    public class CountdownChild : IDisposable
    {
        private int _remaining;
        private readonly int _initial;
        private readonly object _lock = new object();
        private readonly ManualResetEventSlim _gate;
        private CountdownChild _parent;   // 树形级联

        public CountdownChild(int initialCount)
        {
            if (initialCount <= 0) throw new ArgumentOutOfRangeException(nameof(initialCount));
            _remaining = _initial = initialCount;
            _gate = new ManualResetEventSlim(false);
        }

        #region 核心 API

        /// <summary>一次减 N</summary>
        public void Signal(int n = 1)
        {
            if (n <= 0) return;
            int newRem;
            lock (_lock)
            {
                if (_remaining == 0) return;          // 已触发
                _remaining -= n;
                if (_remaining < 0) _remaining = 0;   // 保护性修正
                newRem = _remaining;
            }
            if (newRem == 0) { _gate.Set(); PropagateToParent(); }
        }

        /// <summary>动态加 N（未触发前才允许）</summary>
        public void AddCount(int n = 1)
        {
            if (n <= 0) return;
            lock (_lock)
            {
                if (_gate.IsSet) throw new InvalidOperationException("Already triggered.");
                _remaining += n;
            }
        }

        /// <summary>等待计数归零</summary>
        public void Wait()
        { _gate.Wait(); }

        /// <summary>当前是否已触发</summary>
        public bool IsSet
        { get { lock (_lock) return _remaining == 0; } }

        /// <summary>重置为初始值（可复用对象）</summary>
        public void Reset()
        { lock (_lock) { _remaining = _initial; _gate.Reset(); } }

        #endregion 核心 API

        #region 父子级联

        /// <summary>把当前节点挂到父节点下，形成树形</summary>
        public void AttachToParent(CountdownChild parent)
        {
            if (parent == null) return;
            lock (_lock)
            {
                if (IsSet) { parent.Signal(_remaining); return; }
                _parent = parent;
            }
        }

        private void PropagateToParent()
        {
            var p = _parent;
            if (p != null) p.Signal(_remaining);   // 把剩余量一次性汇报给父
        }

        #endregion 父子级联

        #region IDisposable

        public void Dispose()
        { _gate?.Dispose(); }

        #endregion IDisposable
    }
}