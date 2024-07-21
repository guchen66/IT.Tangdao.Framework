using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public class DateTimeHelper
    {

        #region 时间戳的转换
        public static DateTime DateTime1970 = new DateTime(1970, 1, 1).ToLocalTime();

        /// <summary>
        /// 1970到现在的毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return (long)(DateTime.Now.ToLocalTime() - DateTime1970).TotalSeconds;
        }

        /// <summary>
        /// 1970到指定日期的毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToLocalTime() - DateTime1970).TotalSeconds;
        }

        #endregion
    }
}
