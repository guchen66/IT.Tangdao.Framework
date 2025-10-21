using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Sockets
{
    public static class TangdaoChannelBuilder
    {
        public static TangdaoChannelContext Build(NetMode mode, string connStr)
        {
            var uri = new TangdaoUri(connStr);
            var channel = new TangdaoChannel(mode, uri);
            var request = new TangdaoRequest(channel);
            var response = new TangdaoResponse(channel);
            return new TangdaoChannelContext(channel, request, response);
        }
    }
}