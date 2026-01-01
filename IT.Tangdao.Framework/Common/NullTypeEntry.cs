using IT.Tangdao.Framework.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common
{
    /// <summary>
    /// 空对象模式
    /// </summary>
    public sealed class NullTypeEntry : IRegistrationTypeEntry
    {
        public static readonly NullTypeEntry Instance = new NullTypeEntry();

        private NullTypeEntry()
        {
            Id = 0;
            Key = string.Empty;
            RegisterType = typeof(NullTypeEntry);
        }

        public int? Id { get; }
        public string Key { get; }
        public Type RegisterType { get; }

        public bool IsNull() => true;
    }
}