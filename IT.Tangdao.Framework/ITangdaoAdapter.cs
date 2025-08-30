using IT.Tangdao.Framework.DaoCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    /// <summary>
    /// 容器适配器，当存在第三方容器作为主容器的时候，Tangdao内部的部分接口可以使用
    /// </summary>
    public interface ITangdaoAdapter : ITangdaoContainer
    {
    }
}