using IT.Tangdao.Framework.DaoDtos.Items;
using IT.Tangdao.Framework.DaoMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IT.Tangdao.Framework.DaoSelectors
{
    public class DateTimeSelector
    {
        private static readonly Lazy<DateTimeItem> _dateTimeItem = new Lazy<DateTimeItem>(() => new DateTimeItem());

        public static DateTimeItem Instance => _dateTimeItem.Value;

        // 可选：提供一个静态属性用于绑定
        public static DateTime CurrentDateTime => Instance.CurrentDate;
    }
}