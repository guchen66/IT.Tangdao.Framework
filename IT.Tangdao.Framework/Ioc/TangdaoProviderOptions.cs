using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    public class TangdaoProviderOptions
    {
        /// <summary>
        /// 为解析接口提供默认值
        /// </summary>
        internal static readonly TangdaoProviderOptions Default = new TangdaoProviderOptions();

        /// <summary>
        /// 解析接口是否由容器构建
        /// </summary>
        public bool IsFromContainer { get; set; }

        /// <summary>
        /// 解析接口是否来自子类的实例化
        /// </summary>
        public bool IsFromInstance { get; set; }
    }
}