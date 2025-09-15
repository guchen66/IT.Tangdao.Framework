using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 对XML文件指定类型
    /// </summary>
    public enum DaoXmlType
    {
        /// <summary>
        /// XML文件但内容为空
        /// </summary>
        Empty,

        /// <summary>
        ///<![CDATA[只有XML声明<?xml version="1.0"?>无具体内容]]>
        /// </summary>
        None,

        /// <summary>
        /// 单个根节点且只有一个子节点
        /// </summary>
        Single,

        /// <summary>
        /// 单个根节点但有多个子节点
        /// </summary>
        Multiple
    }
}