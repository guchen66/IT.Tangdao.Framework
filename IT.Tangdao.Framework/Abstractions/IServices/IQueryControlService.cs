using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Abstractions.IServices
{
    public interface IQueryControlService
    {
        /// <summary>
        /// 查询一个面板的布局容器
        /// </summary>
        void QueryableLayoutControl(DependencyObject parent);
    }
}
