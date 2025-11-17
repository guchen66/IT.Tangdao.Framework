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

        // 构造函数
        public TangdaoPool()
        {
            // 使用 FormatterServices.GetUninitializedObject 创建对象
            // 这个方法会创建对象但不调用构造函数，性能较好
            _creator = () => (T)FormatterServices.GetUninitializedObject(typeof(T));
        }

        // 从对象池中租借一个对象
        public T Rent()
        {
            // 尝试从池中取出一个对象
            if (_pool.TryTake(out T item))
                return item;  // 如果池中有可用对象，直接返回

            // 如果池中没有可用对象，创建新的对象
            return _creator();
        }

        // 将对象归还到对象池
        public void Return(T item)
        {
            // 重置对象状态而不是销毁
            // 获取类型 T 的所有可序列化字段
            if (FormatterServices.GetSerializableMembers(typeof(T)) is var fields && fields.Length > 0)
            {
                // 遍历所有字段
                foreach (var field in fields)
                {
                    if (field is FieldInfo fi)
                    {
                        // 将字段重置为默认值
                        fi.SetValue(item, GetDefaultValue(fi.FieldType));
                    }
                }
            }

            // 将重置后的对象放回池中，供后续重用
            _pool.Add(item);
        }

        // 获取类型的默认值
        // 值类型：创建该类型的实例（相当于 default(T)）
        // 引用类型：返回 null
        private static object GetDefaultValue(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}