using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 注册表入口抽象
    /// </summary>
    public interface IRegistrationTypeEntry
    {
        int? Id { get; }

        string Key { get; }

        Type RegisterType { get; }

        bool IsNull();          // 可选：语义级“空”标识
    }
}