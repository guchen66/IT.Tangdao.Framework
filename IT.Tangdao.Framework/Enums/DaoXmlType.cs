using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    public enum DaoXmlType
    {
        Empty,      // XML文件但内容为空
        None,       // 只有XML声明<?xml version="1.0"?>
        Single,     // 单个根节点且只有一个子节点
        Multiple    // 单个根节点但有多个子节点
    }
}