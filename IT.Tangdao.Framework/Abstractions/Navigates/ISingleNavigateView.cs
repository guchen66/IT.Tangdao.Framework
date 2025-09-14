using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Navigates
{
    public interface ISingleNavigateView
    {
        string ViewName { get; }

        /// <summary>
        /// 用于排序
        /// </summary>
        int DisplayOrder { get; }

        /// <summary>
        /// 用于分组
        /// </summary>
        string GroupKey { get; }
    }
}