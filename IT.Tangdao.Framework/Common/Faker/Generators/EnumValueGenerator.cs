
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Utilities;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 枚举生成器：负责所有 <c>enum</c> 类型。
    /// <para>
    /// 从枚举成员中随机取一个，返回枚举值本身（而非名称字符串）——
    /// 属性声明的是枚举类型，赋字符串会在反射赋值时抛异常。
    /// </para>
    /// <para>
    /// 若业务上确实需要"枚举的文本表示"，那只应发生在"属性本身是 string、
    /// 且通过模板声明"的场景，与本生成器无关。
    /// </para>
    /// </summary>
    public sealed class EnumValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>
        /// 判断是否为枚举类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type.IsEnum;
        }

        /// <summary>
        /// 随机取一个枚举成员。
        /// </summary>
        /// <param name="type">目标枚举类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            return FakedataUtils.GetRandomEnumValue(context.Random, type, false);
        }
    }
}
