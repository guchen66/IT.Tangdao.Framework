using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilys
{
    public static class Parsers
    {
        public static readonly Dictionary<Type, Func<string, object>> Table =
            new Dictionary<Type, Func<string, object>>()
            {
                [typeof(int)] = s => int.Parse(s, CultureInfo.InvariantCulture),
                [typeof(bool)] = s => bool.Parse(s),
                [typeof(Uri)] = s => new Uri(s, UriKind.RelativeOrAbsolute),
                // 需要就继续加
            };
    }
}