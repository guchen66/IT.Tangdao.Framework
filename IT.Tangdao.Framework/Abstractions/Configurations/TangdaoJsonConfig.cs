using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    [AutoConfigure(1)]
    internal sealed class TangdaoJsonConfig : ITangdaoConfig
    {
        public ITangdaoOrder Order => new TangdaoOrder(1);   // 也可以直接用属性，双重保险
    }

    // 放 IT.Tangdao.Framework.Abstractions.Configuration 或当前 Implementation 均可
    public sealed class TangdaoConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
        public bool IsAdmin { get; set; }
        public string Role { get; set; }
    }
}