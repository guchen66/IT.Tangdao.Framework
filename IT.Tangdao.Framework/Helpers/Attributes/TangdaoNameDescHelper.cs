using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Contracts;

namespace IT.Tangdao.Framework.Helpers
{
    public class TangdaoNameDescHelper
    {
        private static Type FindUserCustomProvider()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var currentAssemblyName = currentAssembly.GetName();

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm != currentAssembly) // 程序集引用比较
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(string).IsAssignableFrom(t) &&
                           !t.IsInterface &&
                           !t.IsAbstract)
                .FirstOrDefault();
        }
    }
}