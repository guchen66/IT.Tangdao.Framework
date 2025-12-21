using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    public sealed class ServiceCreationContext
    {
        public IServiceEntry Entry { get; set; }
        public IServiceFactory Factory { get; set; }
        public object Key { get; set; }  // 新增
    }
}