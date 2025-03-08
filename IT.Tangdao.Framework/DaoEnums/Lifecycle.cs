using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEnums
{
    public enum Lifecycle
    {
        Transient, // 每次解析时创建新实例
        Singleton, // 只创建一个实例，后续解析返回同一个实例
        Scope
    }
}