using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 集中分隔符
    /// </summary>
    internal static class Separators
    {
        internal static readonly char[] Semicolon = new[] { ';' };
        internal static readonly char[] Line = new[] { '\r', '\n' };
        internal static readonly char[] Point = new[] { '.' };
        internal static readonly char[] Space = new[] { ' ' };
        internal static readonly char[] PathSplit = new[] { Path.PathSeparator };
        // 以后再加...
    }
}