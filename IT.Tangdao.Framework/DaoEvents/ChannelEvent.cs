using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEvents
{
    /// <summary>
    /// 同步数据传输，进行两个类之间简单的数据交互
    /// </summary>
    public static class ChannelEvent
    {
        private static readonly ConcurrentDictionary<string, string> _localValues = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, Delegate> _actions = new ConcurrentDictionary<string, Delegate>();

        public static void SetLocalValue(string name, string value)
        {
            _localValues[name] = value;
        }

        public static string GetLocalValue(string name)
        {
            _localValues.TryGetValue(name, out var value);
            return value;
        }

        // 存储Action的方法
        public static void SetLocalAction(string name, Action action)
        {
            _actions[name] = action;
        }

        // 执行存储的Action的方法
        public static void GetLocalAction(string name)
        {
            if (_actions.TryGetValue(name, out var action))
            {
                // 确保存储的是Action类型
                if (action is Action)
                {
                    (action as Action)?.Invoke();
                }
            }
        }
    }

    public class WriteEvent
    {
        public static void Write()
        {
            ChannelEvent.SetLocalValue("name", "Hello");
        }
    }

    public class ReadEvent
    {
        public static void Read()
        {
            var value = ChannelEvent.GetLocalValue("name");
            Console.WriteLine(value); // 输出 Hello
        }
    }

    public class First
    {
        public static void Usage()
        {
            WriteEvent.Write();
        }
    }
}
