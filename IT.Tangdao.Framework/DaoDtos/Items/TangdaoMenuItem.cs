using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDtos.Items
{
    /// <summary>
    /// 用于简单的标题书写
    /// </summary>
    public class TangdaoMenuItem : IMenuItem
    {
        public int Id { get; set; }

        public string MenuName { get; set; }
    }

    public interface IMenuItem
    {
        int Id { get; set; }

        string MenuName { get; set; }
    }
}