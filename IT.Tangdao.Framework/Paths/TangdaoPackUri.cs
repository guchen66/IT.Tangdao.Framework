using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    /// <summary>
    /// IUriContext解析数据
    /// </summary>
    public sealed class TangdaoPackUri
    {
        /// <summary>
        /// 完整路径pack://application:,,,/App;component/views/xxx.xaml
        /// </summary>
        public string FullUri { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// .xaml解析数据
        /// </summary>
        public string ComponentFile { get; set; }
    }
}