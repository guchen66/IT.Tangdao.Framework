
using IT.Tangdao.Framework.Abstractions.Contracts;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// Guid 生成器：负责 <c>Guid</c> 类型。
    /// <para>
    /// 直接调用 <c>Guid.NewGuid()</c> 产出全局唯一标识。
    /// </para>
    /// <para>
    /// 【修正说明】旧实现用 <c>typeof(Guid).GetProperty("NewGuid") != null</c> 做探测，
    /// 但 <c>NewGuid</c> 是静态方法而非属性，反射恒返回 null，
    /// 导致所有 Guid 属性都被赋成 <c>Guid.Empty</c>，属于历史缺陷。本版改为直接生成。
    /// </para>
    /// </summary>
    public sealed class GuidValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>
        /// 本生成器只负责 <c>Guid</c>。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(Guid);
        }

        /// <summary>
        /// 生成一个新的 Guid。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            return Guid.NewGuid();
        }
    }
}
