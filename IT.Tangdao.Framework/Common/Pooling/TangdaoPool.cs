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
    public class TangdaoPool<T> where T : class
    {
        private readonly ConcurrentBag<T> _pool = new ConcurrentBag<T>();
        private readonly Func<T> _creator;

        public TangdaoPool()
        {
            _creator = () => (T)FormatterServices.GetUninitializedObject(typeof(T));
        }

        public T Rent()
        {
            if (_pool.TryTake(out T item))
                return item;

            return _creator();
        }

        public void Return(T item)
        {
            // 重置对象状态而不是销毁
            if (FormatterServices.GetSerializableMembers(typeof(T)) is var fields && fields.Length > 0)
            {
                foreach (var field in fields)
                {
                    if (field is FieldInfo fi)
                    {
                        // 将字段重置为默认值
                        fi.SetValue(item, GetDefaultValue(fi.FieldType));
                    }
                }
            }
            _pool.Add(item);
        }

        private object GetDefaultValue(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}