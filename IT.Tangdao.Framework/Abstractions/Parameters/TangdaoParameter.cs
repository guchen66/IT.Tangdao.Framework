using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public class TangdaoParameter : ITangdaoParameter
    {
        // 存储参数
        private readonly ConcurrentDictionary<string, ITangdaoValue> _parameters = new ConcurrentDictionary<string, ITangdaoValue>();

        // 存储命令
        private readonly ConcurrentDictionary<string, Delegate> _commands = new ConcurrentDictionary<string, Delegate>();

        // 默认类型注册信息的键名
        private const string DefaultRegistrationTypeEntryKey = "RegistrationTypeEntry";

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

        // 添加类型注册信息（使用默认键名）
        /// <summary>
        /// 添加类型注册信息（使用默认键名）
        /// </summary>
        /// <param name="registrationTypeEntry">类型注册信息</param>
        /// <remarks>
        /// 注意：如果多次调用此方法，会覆盖之前添加的类型注册信息
        /// 如果需要添加多个类型注册信息，请使用带自定义键名的重载方法
        /// </remarks>
        public void AddRegistrationTypeEntry(IRegistrationTypeEntry registrationTypeEntry)
        {
            AddRegistrationTypeEntry(DefaultRegistrationTypeEntryKey, registrationTypeEntry);
        }

        /// <summary>
        /// 添加类型注册信息（使用自定义键名）
        /// </summary>
        /// <param name="key">自定义键名，用于标识不同的类型注册信息</param>
        /// <param name="registrationTypeEntry">类型注册信息</param>
        /// <remarks>
        /// 使用自定义键名可以在同一个TangdaoParameter实例中存储多个类型注册信息
        /// 适用于需要传输多个不同类型注册信息的场景
        /// </remarks>
        public void AddRegistrationTypeEntry(string key, IRegistrationTypeEntry registrationTypeEntry)
        {
            Add(key, registrationTypeEntry);
        }

        // 获取类型注册信息（使用默认键名）
        /// <summary>
        /// 获取类型注册信息（使用默认键名）
        /// </summary>
        /// <returns>类型注册信息，如果不存在则返回null</returns>
        public IRegistrationTypeEntry GetRegistrationTypeEntry()
        {
            return GetRegistrationTypeEntry(DefaultRegistrationTypeEntryKey);
        }

        // 获取类型注册信息（使用自定义键名）
        /// <summary>
        /// 获取类型注册信息（使用自定义键名）
        /// </summary>
        /// <param name="key">自定义键名</param>
        /// <returns>类型注册信息，如果不存在则返回null</returns>
        public IRegistrationTypeEntry GetRegistrationTypeEntry(string key)
        {
            return Get<IRegistrationTypeEntry>(key);
        }
    }
}