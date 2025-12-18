using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Commands
{
    public sealed class HandlerTable : IHandlerTable
    {
        private readonly ConcurrentDictionary<string, Action> _map = new ConcurrentDictionary<string, Action>();

        private readonly ConcurrentDictionary<string, Action<HandlerResult>> _mapWithArgs = new ConcurrentDictionary<string, Action<HandlerResult>>();

        public void Register(string key, Action action)
        {
            _map.AddOrUpdate(key, action, (k, oldValue) => action);
        }

        public void Register(string key, Action<HandlerResult> action)
        {
            _mapWithArgs.AddOrUpdate(key, action, (k, oldValue) => action);
        }

        public Action GetHandler(string key)
        {
            return _map.TryGetValue(key, out var cmd) ? cmd : null;
        }

        public Action<HandlerResult> GetResultHandler(string key)
        {
            return _mapWithArgs.TryGetValue(key, out var cmd) ? cmd : null;
        }
    }
}