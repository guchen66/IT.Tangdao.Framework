
using IT.Tangdao.Framework.Abstractions.Contracts;
using System;
using System.Reflection;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 嵌套对象生成器：负责"其它生成器都不认领"的复杂类型（自定义类、结构体）。
    /// <para>
    /// 之所以放在注册表<b>最后</b>注册，是因为它承担兜底角色：任何已被
    /// <c>string / 整数 / 浮点 / bool / 日期 / 枚举 / Guid</c> 认领的类型都不会走到这里。
    /// </para>
    /// <para>
    /// 它通过"实例化 → 遍历可写属性 → 交给规则链递归填充"的方式工作。
    /// 子属性走规则链而不是直接调生成器，是为了让子属性上声明的
    /// <c>[TangdaoFake]</c>（长度、范围、模板等）同样生效。
    /// </para>
    /// </summary>
    public sealed class NestedObjectValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>
        /// 判断是否为需要递归填充的复杂类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            // 接口与抽象类无法实例化，直接放弃
            if (type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            // 已由专用生成器负责的类型不接手
            if (type.IsPrimitive || type.IsEnum)
            {
                return false;
            }

            if (type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(Guid))
            {
                return false;
            }

            // 可空类型在解包阶段已处理，不应出现在这里
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return false;
            }

            return type.IsClass || type.IsValueType;
        }

        /// <summary>
        /// 实例化目标类型并递归填充其公共可写属性。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        /// <returns>填充后的实例；达到深度上限或无参构造不可用时返回 null</returns>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            // 深度保护：达到上限即停止下沉，返回 null 由调用方决定是否接受
            // （引用类型属性可以接受 null，值类型属性由调用方跳过赋值）
            if (context.Depth >= context.Options.MaxNestedDepth)
            {
                return null;
            }

            object instance;
            try
            {
                instance = Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                // 没有无参构造函数（例如只有带参构造的实体）：无法安全构造，放弃
                return null;
            }

            if (instance == null)
            {
                return null;
            }

            // 子属性在更深一层上下文中生成，深度用于阻断循环引用
            TangdaoGeneratorContext childContext = context.CreateChild();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                // 只处理可写、非索引器属性
                if (!property.CanWrite || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                // 单个子属性失败不应让整个对象生成失败，因此这里吞掉异常并跳过该属性
                try
                {
                    object value = FakeRuleChain.ResolvePropertyValue(property, childContext);

                    // 值类型属性无法接受 null，跳过赋值保留默认值
                    if (value == null && property.PropertyType.IsValueType)
                    {
                        continue;
                    }

                    property.SetValue(instance, value, null);
                }
                catch (Exception)
                {
                    // 忽略该属性，继续处理下一个
                }
            }

            return instance;
        }
    }
}
