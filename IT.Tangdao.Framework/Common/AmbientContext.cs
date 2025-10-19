using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 线程本地存储 + 异步上下文双兼容，0 字典查找路径
    /// 支持值类型零 GC，引用类型 CallContext 回落
    /// </summary>
    public class AmbientContext
    {
        /* ---------- FastTLS：值类型专用，0 字典 ---------- */

        private static class Slot<T> where T : struct
        {
            // 每个 T 占用一条线程静态字段 → 0 查找
            [ThreadStatic]
            private static T _value;

            [ThreadStatic]
            internal static bool _initialized;  // 明确的初始化标志

            public static T Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public static bool IsInitialized => _initialized;

            public static void Clear()
            {
                _value = default(T);
                _initialized = false;
            }
        }

        /* ---------- ScopedTLS：引用类型/需要流动上下文 ---------- */
        private const string SlotPrefix = "__AmbientContext_";

        private static string KeyOf<T>() => SlotPrefix + typeof(T).FullName;

        /// <summary>
        /// 获取或创建值类型线程/上下文本地存储
        /// </summary>
        public static T GetOrCreate<T>(Func<T> factory) where T : struct
        {
            if (!Slot<T>.IsInitialized)  // 明确的初始化检查
            {
                Slot<T>.Value = factory();
            }
            return Slot<T>.Value;
        }

        public static void Set<T>(T value) where T : struct
        {
            Slot<T>.Value = value;
            Slot<T>._initialized = true;
        }

        public static T Get<T>() where T : struct
        {
            return Slot<T>.Value;
        }

        public static void Clear<T>() where T : struct
        {
            Slot<T>.Clear();
        }

        /* ---------- 引用类型版本：CallContext 回落 ---------- */

        public static T GetCurrent<T>() where T : class
        {
            // return CallContext.LogicalGetData(KeyOf<T>()) as T;
            return GetCurrentCore(KeyOf<T>()) as T;
        }

        public static void SetCurrent<T>(T value) where T : class
        {
            // CallContext.LogicalSetData(KeyOf<T>(), value);
            SetCurrentCore(KeyOf<T>(), value);
        }

        public static void ClearCurrent<T>() where T : class
        {
            CallContext.FreeNamedDataSlot(KeyOf<T>());
        }

        /* ---------- 具名槽 ---------- */

        public static void SetCurrent<T>(string name, T value) where T : class
            => SetCurrentCore(KeyOf<T>() + ":" + name, value);

        public static T GetCurrent<T>(string name) where T : class
            => GetCurrentCore(KeyOf<T>() + ":" + name) as T;

        public static void ClearCurrent<T>(string name) where T : class
            => CallContext.FreeNamedDataSlot(KeyOf<T>() + ":" + name);

        /* ---------- 唯一底层 ---------- */

        private static void SetCurrentCore(string key, object value)
            => CallContext.LogicalSetData(key, value);

        private static object GetCurrentCore(string key)
            => CallContext.LogicalGetData(key);
    }
}