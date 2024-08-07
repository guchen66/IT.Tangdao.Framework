using IT.Tangdao.Framework.DaoDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ObjectInfoExtension
    {
        /// <summary>
        /// 假如对象构造函数没有参数，可以直接调用生成
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static TType TryGetInstanceObject<TType>()
        {
            return (TType)Activator.CreateInstance(typeof(TType));
        }


        /// <summary>
        /// 假如对象构造函数存在参数，可以直接调用生成
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static TType TryGetInstanceObject<TType>(object[] objects=null)
        {
            if (objects.Length > 1)
            {
                foreach (object obj in objects)
                {
                    Type type=obj.GetType();
                    TryGetInstanceObject<TType>();
                }
               
            }

            return (TType)Activator.CreateInstance(typeof(TType),objects);
        }

        /// <summary>
        /// 根据Name取调用
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TType TryGetInstanceObject<TType>(string name)
        {
            ObjectHandle handle=Activator.CreateInstance(name, name);
            return (TType)Activator.CreateInstance(typeof(TType));
        }
    }
}
