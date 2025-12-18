using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Commands;

namespace IT.Tangdao.Framework.EventArg
{
    public sealed class HandlerTableEventArgs : TangdaoEventArgs
    {
        public IHandlerTable HandlerTable { get; set; }
        public string Key { get; set; }

        public HandlerTableEventArgs(string key, IHandlerTable handlerTable)
        {
            Key = key;
            HandlerTable = handlerTable;
        }
    }
}