using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public class TangdaoParameter : ITangdaoParameter
    {
        // 存储参数
        private readonly ConcurrentDictionary<string, ITangdaoValue> _parameters = new ConcurrentDictionary<string, ITangdaoValue>();

        // 存储命令
        private readonly ConcurrentDictionary<string, Delegate> _commands = new ConcurrentDictionary<string, Delegate>();

        // 添加参数
        public void Add<T>(string key, T value)
        {
            _parameters[key] = new TangdaoValue<T>(value);
        }

        // 获取参数
        public T Get<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var box) && box is TangdaoValue<T> b)
                return b.Value;                // 无拆箱
            return default;
        }

        // 添加无参数、无返回值的命令
        public void AddCommand(string key, Action command)
        {
            _commands[key] = command;
        }

        // 添加带参数、无返回值的命令
        public void AddCommand<T>(string key, Action<T> command)
        {
            _commands[key] = command;
        }

        // 添加带参数、有返回值的命令
        public void AddCommand<T, TResult>(string key, Func<T, TResult> command)
        {
            _commands[key] = command;
        }

        // 执行无参数、无返回值的命令
        public void ExecuteCommand(string key)
        {
            if (_commands.TryGetValue(key, out var command) && command is Action action)
            {
                action.Invoke();
            }
            else
            {
                throw new InvalidOperationException($"Command '{key}'未找到或类型无效.");
            }
        }

        // 执行带参数、无返回值的命令
        public void ExecuteCommand<T>(string key, T parameter)
        {
            if (_commands.TryGetValue(key, out var command) && command is Action<T> action)
            {
                action.Invoke(parameter);
            }
            else
            {
                throw new InvalidOperationException($"Command '{key}' 未找到或类型无效.");
            }
        }

        // 执行带参数、有返回值的命令
        public TResult ExecuteCommand<T, TResult>(string key, T parameter)
        {
            if (_commands.TryGetValue(key, out var command) && command is Func<T, TResult> func)
            {
                return func.Invoke(parameter);
            }
            else
            {
                throw new InvalidOperationException($"Command '{key}' 未找到或类型无效.");
            }
        }
    }
}