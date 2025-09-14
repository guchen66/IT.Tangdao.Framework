using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    internal sealed class StringHelper
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <summary>
        ///  null、空、空白 或 "NULL"（忽略大小写）都视为空
        /// </summary>
        public static bool IsNullOrEmptyToken(string value)
            => string.IsNullOrWhiteSpace(value) ||
               value.Equals("NULL", StringComparison.OrdinalIgnoreCase);
    }
}