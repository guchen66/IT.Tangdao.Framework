using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Paths;

namespace IT.Tangdao.Framework.Reflection
{
    internal class TypeContext
    {
        /// <summary>
        /// 类绝对路径
        /// </summary>
        public AbsolutePath AbsolutePath { get; set; }

        /// <summary>
        /// 类的相对路径
        /// </summary>
        public RelativePath RelativePath { get; set; }

        /// <summary>
        /// 类的命名空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 类的完整名称（命名空间.类型名）
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 类的元数据
        /// </summary>
        public TypeMetadata Metadata { get; set; }

        /// <summary>
        /// 类的特性信息类
        /// </summary>
        public AttributeInfo AttributeInfo { get; set; }
    }
}