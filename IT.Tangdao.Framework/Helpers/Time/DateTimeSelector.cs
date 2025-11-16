using System;
using IT.Tangdao.Framework.Infrastructure;

namespace IT.Tangdao.Framework.Helpers
{
    public class DateTimeSelector
    {
        private static readonly Lazy<TangdaoClock> _dateTimeItem = new Lazy<TangdaoClock>(() => new TangdaoClock());

        public static TangdaoClock Instance => _dateTimeItem.Value;

        // 可选：提供一个静态属性用于绑定
        public static DateTime CurrentDateTime => Instance.CurrentDate;
    }
}