using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    public class EventTransmit : IEventTransmit
    {
        private readonly Dictionary<Type, List<Action<object>>> _eventObjects = new Dictionary<Type, List<Action<object>>>();

        public void Publish<T>(object obj = null) where T : DaoEventBase, new()
        {
            var eventType = typeof(T);

            if (_eventObjects.ContainsKey(eventType))
            {
                T eventObj=obj is T ? (T)obj : Activator.CreateInstance<T>();
                foreach (var action in _eventObjects[eventType])
                {
                    action(eventObj);
                }
               
            }
        }

        public void Subscribe<T>(Action<object> executeMethod) where T : DaoEventBase, new()
        {
            var eventType = typeof(T);
            if (!_eventObjects.ContainsKey(eventType))
            {
                _eventObjects[eventType] = new List<Action<object>>();

            }
            _eventObjects[eventType].Add(executeMethod);
        }

        public void UnSubscribe<T>(Action<object> executeMethod) where T : DaoEventBase, new()
        {
            var eventType = typeof(T);
            if (!_eventObjects.ContainsKey(eventType))
            {
                _eventObjects[eventType].Remove(executeMethod);

            }
        }
    }
}
