using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Pooling
{
    /// <summary>
    /// 泛型对象池类，T 必须是引用类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TangdaoPool<T> where T : class
    {
        // 线程安全的对象容器，使用 ConcurrentBag 来支持多线程并发访问
        private readonly ConcurrentBag<T> _pool = new ConcurrentBag<T>();

        // 对象创建委托，用于在池中无可用对象时创建新对象
        private readonly Func<T> _creator;

        // 类型 T 的所有可写公共属性缓存
        private readonly PropertyInfo[] _writableProperties;

        // 构造函数
        public TangdaoPool()
        {
            // 使用 FormatterServices.GetUninitializedObject 创建对象
            // 这个方法会创建对象但不调用构造函数，性能较好
            _creator = () => (T)FormatterServices.GetUninitializedObject(typeof(T));

            // 缓存可写公共属性，用于更彻底的对象重置
            _writableProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true)
                .ToArray();
        }

        /// <summary>
        /// 从对象池中租借一个对象
        /// </summary>
        /// <returns>池中的对象或新创建的对象</returns>
        public T Rent()
        {
            // 尝试从池中取出一个对象
            if (_pool.TryTake(out T item))
                return item;  // 如果池中有可用对象，直接返回

            // 如果池中没有可用对象，创建新的对象
            return _creator();
        }

        /// <summary>
        /// 将对象归还到对象池
        /// </summary>
        /// <param name="item">要归还的对象</param>
        public void Return(T item)
        {
            // 重置对象状态而不是销毁
            ResetObjectState(item);

            // 将重置后的对象放回池中，供后续重用
            _pool.Add(item);
        }

        /// <summary>
        /// 重置对象状态
        /// </summary>
        /// <param name="item">要重置状态的对象</param>
        private void ResetObjectState(T item)
        {
            //  重置所有可写公共属性
            foreach (var property in _writableProperties)
            {
                property.SetValue(item, GetDefaultValue(property.PropertyType));
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>该类型的默认值</returns>
        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}