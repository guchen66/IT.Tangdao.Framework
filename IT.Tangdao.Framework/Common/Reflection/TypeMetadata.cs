using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Reflection
{
    public class TypeMetadata
    {
        /// <summary>
        /// 此类是否单例
        /// </summary>
        public bool IsSingleton { get; }

        /// <summary>
        /// 是否可继承
        /// </summary>
        public bool IsSealed { get; }

        /// <summary>
        /// 此类存在的父类
        /// </summary>
        public Type BaseClass { get; }

        /// <summary>
        /// 此类存在的接口
        /// </summary>
        public IReadOnlyList<Type> Interfaces { get; }

        public TypeMetadata()
        {
        }

        public TypeMetadata(Type baseClass, IReadOnlyList<Type> interfaces)
        {
            BaseClass = baseClass;
            Interfaces = interfaces ?? Array.Empty<Type>();
        }
    }
}