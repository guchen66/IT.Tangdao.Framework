using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    public sealed class KeyMessageEventArgs : TangdaoEventArgs
    {
        public string Key { get; }

        public MessageEventArgs MessageEventArgs { get; set; }

        public KeyMessageEventArgs(string key, MessageEventArgs messageEventArgs)
        {
            Key = key;
            MessageEventArgs = messageEventArgs;
        }
    }
}