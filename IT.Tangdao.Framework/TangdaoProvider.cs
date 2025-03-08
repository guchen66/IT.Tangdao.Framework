using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.DaoDtos.Globals;
using IT.Tangdao.Framework.DaoDtos.Options;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoProvider : ITangdaoProvider
    {
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public object Resolve(Type type, params object[] impleType)
        {
            return Activator.CreateInstance(type, impleType);
        }
    }
}