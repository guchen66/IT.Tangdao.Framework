using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilys
{
    /// <summary>
    /// 对象创建工具类
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// 创建无参构造函数的对象实例
        /// </summary>
        public static TType CreateInstance<TType>() where TType : class
        {
            return Activator.CreateInstance<TType>();
        }

        /// <summary>
        /// 创建带参数构造函数的对象实例
        /// </summary>
        public static TType CreateInstance<TType>(params object[] args) where TType : class
        {
            if (args == null || args.Length == 0)
                return Activator.CreateInstance<TType>();

            return (TType)Activator.CreateInstance(typeof(TType), args);
        }

        /// <summary>
        /// 通过类型名称创建对象实例（适用于延迟加载）
        /// </summary>
        public static TType CreateInstanceFromName<TType>(string typeName) where TType : class
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));

            Type type = Type.GetType(typeName);
            if (type == null)
                throw new TypeLoadException($"无法找到类型: {typeName}");

            return (TType)Activator.CreateInstance(type);
        }

        /// <summary>
        /// 运行时拷贝对象属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <returns></returns>
        public static Func<TSource, TTarget> CreateMap<TSource, TTarget>()
        {
            var p = Expression.Parameter(typeof(TSource), "instance");
            var body = Expression.MemberInit(Expression.New(typeof(TTarget)), typeof(TTarget).GetProperties().Select(d => Expression.Bind(d, Expression.Property(p, d.Name))));
            return Expression.Lambda<Func<TSource, TTarget>>(body, p).Compile();
        }

        /// <summary>
        /// 尝试创建对象实例，失败返回null而不是抛出异常
        /// </summary>
        public static TType TryCreateInstance<TType>() where TType : class
        {
            try
            {
                return Activator.CreateInstance<TType>();
            }
            catch
            {
                return null;
            }
        }
    }
}