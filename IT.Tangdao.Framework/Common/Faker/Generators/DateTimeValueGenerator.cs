
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Utilities;
using System;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 日期族生成器：负责 <c>DateTime</c> 与 <c>DateTimeOffset</c>。
    /// <para>
    /// 取值区间来自 <see cref="TangdaoFakerOptions.MinDate"/> / <see cref="TangdaoFakerOptions.MaxDate"/>，
    /// 库默认是 1990-01-01 ~ 当天，并按日、时、分、秒逐级随机，产出带时分秒的完整时间，
    /// 而不是只精确到天的日期。
    /// </para>
    /// <para>
    /// 把区间做成配置项而不是写死常量，是为了让调用方能按业务场景收窄范围
    /// （例如"只生成最近一年的订单时间"）。
    /// </para>
    /// </summary>
    public sealed class DateTimeValueGenerator : ITangdaoValueGenerator
    {
        /// <summary>
        /// 判断是否为日期族类型。
        /// </summary>
        /// <param name="type">目标类型</param>
        public bool CanGenerate(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

        /// <summary>
        /// 在配置区间内生成一个随机时间点。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="context">生成上下文</param>
        public object Generate(Type type, TangdaoGeneratorContext context)
        {
            DateTime value = FakedataUtils.GenerateRandomDateTime(
                context.Random,
                context.Options.MinDate,
                context.Options.MaxDate);

            // DateTimeOffset 需要显式包装，否则赋值时会因类型不匹配抛异常
            if (type == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(value);
            }

            return value;
        }
    }
}
