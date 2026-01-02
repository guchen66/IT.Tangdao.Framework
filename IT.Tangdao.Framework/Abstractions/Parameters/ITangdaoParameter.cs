using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface ITangdaoParameter
    {
        // 添加参数
        void Add<T>(string key, T value);

        // 获取参数
        T Get<T>(string key);

        // 添加无参数、无返回值的命令
        void AddCommand(string key, Action command);

        // 添加带参数、无返回值的命令
        void AddCommand<T>(string key, Action<T> command);

        // 添加带参数、有返回值的命令
        void AddCommand<T, TResult>(string key, Func<T, TResult> command);

        // 执行无参数、无返回值的命令
        void ExecuteCommand(string key);

        // 执行带参数、无返回值的命令
        void ExecuteCommand<T>(string key, T parameter);

        // 执行带参数、有返回值的命令
        TResult ExecuteCommand<T, TResult>(string key, T parameter);

        // 添加类型注册信息（使用默认键名）
        void AddRegistrationTypeEntry(IRegistrationTypeEntry registrationTypeEntry);

        // 添加类型注册信息（使用自定义键名）
        void AddRegistrationTypeEntry(string key, IRegistrationTypeEntry registrationTypeEntry);

        // 获取类型注册信息（使用默认键名）
        IRegistrationTypeEntry GetRegistrationTypeEntry();

        // 获取类型注册信息（使用自定义键名）
        IRegistrationTypeEntry GetRegistrationTypeEntry(string key);
    }
}