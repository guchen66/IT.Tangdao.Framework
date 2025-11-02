using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 扩展Loaded方法
    /// </summary>
    public interface IViewReady
    {
        void OnViewLoaded();
    }
}