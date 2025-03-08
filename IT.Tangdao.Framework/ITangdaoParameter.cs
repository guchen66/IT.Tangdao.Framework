using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
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
    }
}