using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Messaging;

namespace IT.Tangdao.Framework.Events
{
    public class MessageEventArgs : TangdaoEventArgs
    {
        public MessageContext Context { get; set; }

        public bool Cancel { get; set; }

        public MessageEventArgs(MessageContext context)
        {
            Context = context;
        }
    }
}