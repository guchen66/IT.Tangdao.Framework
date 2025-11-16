using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public sealed class TangdaoRequest : TangdaoParameter, ITangdaoRequest
    {
        public string Command => "ReadReg";           // 固化 key
        public TaskCompletionSource<object> ReplySource { get; } = new TaskCompletionSource<object>();

        // 强类型属性，避免 magic string
        public byte Address
        {
            get => Get<byte>("Addr");
            set => Add("Addr", value);
        }

        public ushort Result
        {
            get => Get<ushort>("Result");
            set => Add("Result", value);
        }
    }
}