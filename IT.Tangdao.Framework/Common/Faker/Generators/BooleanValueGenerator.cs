
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Utilities;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 布尔值生成器：负责 <c>bool</c> 类型。
    /// <para>产出等概率的 true / false。本类不持有任何状态，多个线程共用同一实例是安全的。</para>
    /// </summary>
    public sealed class BooleanValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>
        /// 本生成器只负责 <c>bool</c>。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(bool);
        }

        /// <summary>
        /// 生成一个随机布尔值。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            return FakedataUtils.GetRandomBoolean(context.Random);
        }
    }
}
