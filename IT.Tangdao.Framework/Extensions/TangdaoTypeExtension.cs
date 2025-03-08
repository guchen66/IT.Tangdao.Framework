using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoTypeExtension
    {
        /// <summary>
        ///  一个类是否具有无参构造器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHasConstructor(this Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            foreach (ConstructorInfo constructor in constructors)
            {
                if (constructor.GetParameters().Count() == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 一个类是否实现接口T
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool IsHasInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        /// <summary>
        /// 一个类是否有特性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public static bool IsHasAttribute(this Type type, Type attributeType)
        {
            return type.IsDefined(attributeType, false);
        }

        /// <summary>
        /// 一个类子类是否是
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsHasSon(this Type type, Type baseType)
        {
            return type.IsSubclassOf(baseType);
        }

        public static object IsConverter(this Type type, string name)
        {
            if (type.Name == name)
            {

            }
            ;
            return default;
        }
    }
}
