
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Utilities;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 字符串生成器：负责 <c>string</c> 类型的默认造值。
    /// <para>
    /// 默认产出 6 位随机字母数字串。若属性上声明了 <c>[TangdaoFake(Length = n)]</c>，
    /// 会由更高优先级的 <c>LengthRule</c> 抢先接手，不会走到这里。
    /// </para>
    /// </summary>
    public sealed class StringValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>未声明长度时使用的默认字符串长度。</summary>
        private const int DefaultLength = 6;

        /// <summary>
        /// 本生成器只负责 <c>string</c>。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(string);
        }

        /// <summary>
        /// 生成一个默认长度的随机字符串。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            return FakedataUtils.GenerateRandomString(context.Random, DefaultLength);
        }
    }
}
