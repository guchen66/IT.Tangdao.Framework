using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class TangdaoParameter : ITangdaoParameter
    {
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private Dictionary<string, Action> _commands = new Dictionary<string, Action>();
        public void Add<T>(string key, T value)
        {
            _parameters[key] = value;
        }

        public T Get<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default(T);
        }

        public void AddCommand(string key, Action command)
        {
            _commands[key] = command;
        }

        public void ExecuteCommand(string key)
        {
            if (_commands.TryGetValue(key, out var command) && command != null)
            {
                command.Invoke();
            }
        }
    }
}
