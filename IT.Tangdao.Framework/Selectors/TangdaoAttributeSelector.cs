using IT.Tangdao.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Selectors
{
    /// <summary>
    /// 扫描程序集的所有特性的类
    /// </summary>
    public class TangdaoAttributeSelector
    {
        /// <summary>
        /// 寻找ViewToViewModelAttribute特性所在类
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<Type> FindViewToViewModelAttribute(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => type.GetCustomAttributes<ViewToViewModelAttribute>(false).Any());
        }
    }
}