using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 浅拷贝快速复制类
    /// 使用表达式树生成高效的克隆代码，比反射克隆性能更高
    /// </summary>
    /// <typeparam name="T">要克隆的对象类型</typeparam>
    public static class CloneGenerator<T>
    {
        /// <summary>
        /// 预编译的克隆函数，只在第一次使用时生成，之后直接调用
        /// </summary>
        private static readonly Func<T, T> _cloner = GenerateCloner();

        /// <summary>
        /// 生成克隆函数的核心方法
        /// 使用表达式树创建高效的浅拷贝逻辑
        /// </summary>
        /// <returns>编译好的克隆函数</returns>
        private static Func<T, T> GenerateCloner()
        {
            // 创建参数表达式，代表源对象
            var sourceParam = Expression.Parameter(typeof(T), "source");

            // 获取所有需要克隆的公共属性
            // 过滤条件：
            // 1. 有 IgnoreAttribute 的属性忽略
            // 2. 必须有 getter 和 setter
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pr =>
                    !pr.IsDefined(typeof(IgnoreAttribute), false) && // 忽略带有 IgnoreAttribute 的属性
                    pr.CanRead && pr.CanWrite); // 必须可读可写

            // 生成属性绑定表达式：target.Property = source.Property
            var bindings = properties.Select(pr =>
                Expression.Bind(pr, Expression.Property(sourceParam, pr)));

            // 生成对象初始化表达式：new T { Property1 = source.Property1, Property2 = source.Property2, ... }
            var memberInit = Expression.MemberInit(
                Expression.New(typeof(T)), // 调用无参构造函数
                bindings);

            // 编译为委托：Func<T, T>，接收源对象返回克隆对象
            return Expression.Lambda<Func<T, T>>(memberInit, sourceParam).Compile();
        }

        /// <summary>
        /// 克隆对象的公共方法
        /// 调用预编译的克隆函数，实现高效浅拷贝
        /// </summary>
        /// <param name="src">要克隆的源对象</param>
        /// <returns>克隆后的对象</returns>
        public static T Clone(T src) => _cloner(src);
    }
}