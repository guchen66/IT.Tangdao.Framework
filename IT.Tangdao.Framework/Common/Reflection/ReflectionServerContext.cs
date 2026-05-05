using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common.Reflection
{
    public class ReflectionServerContext
    {
        internal readonly Type[] _types;

        public ReflectionServerContext(params Type[] types)
        {
            _types = types;
        }

        public Type FindTypeByName(string typeName)
        {
            return null;
        }
    }
}