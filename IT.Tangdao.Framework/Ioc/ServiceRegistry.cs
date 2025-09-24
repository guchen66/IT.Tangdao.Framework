using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 默认注册表，内部用读写锁保障线程安全。
    /// </summary>
    public sealed class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, IServiceEntry> _dict = new Dictionary<Type, IServiceEntry>();
        private readonly Dictionary<(Type serviceType, object key), IServiceEntry> _keyedDict = new Dictionary<(Type, object), IServiceEntry>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void Add(IServiceEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            _lock.EnterWriteLock();
            try
            {
                _dict[entry.ServiceType] = entry;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IServiceEntry GetEntry(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            _lock.EnterReadLock();
            try
            {
                _dict.TryGetValue(serviceType, out var e);
                return e;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IReadOnlyList<IServiceEntry> GetAllEntries()
        {
            _lock.EnterReadLock();
            try
            {
                // 快照，避免外部遍历时被修改
                return _dict.Values.ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void AddKeyed(IServiceEntry entry, object key)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (key == null) throw new ArgumentNullException(nameof(key));

            _lock.EnterWriteLock();
            try
            {
                _keyedDict[(entry.ServiceType, key)] = entry;
            }
            finally { _lock.ExitWriteLock(); }
        }

        public IServiceEntry GetKeyedEntry(Type serviceType, object key)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (key == null) throw new ArgumentNullException(nameof(key));

            _lock.EnterReadLock();
            try
            {
                _keyedDict.TryGetValue((serviceType, key), out var e);
                return e;
            }
            finally { _lock.ExitReadLock(); }
        }
    }
}