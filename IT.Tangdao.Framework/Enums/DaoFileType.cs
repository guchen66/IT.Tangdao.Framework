using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 常用的文件类型枚举。
    /// </summary>
    public enum DaoFileType
    {
        /// <summary>未指定的文件类型。</summary>
        None,

        /// <summary>纯文本文件。</summary>
        Txt,

        /// <summary>XML配置文件。</summary>
        Xml,

        /// <summary>Excel Open XML 格式文件。</summary>
        Xlsx,

        /// <summary>WPF XAML 界面文件。</summary>
        Xaml,

        /// <summary>JSON 数据文件。</summary>
        Json,

        /// <summary>应用程序配置文件。</summary>
        Config
    }
}