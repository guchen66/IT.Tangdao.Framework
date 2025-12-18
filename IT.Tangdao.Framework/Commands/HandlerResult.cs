using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Commands
{
    public class HandlerResult
    {
        public BackResult Result { get; set; }

        public string Name { get; set; }

        public ITangdaoParameter Parameter { get; set; }
    }
}