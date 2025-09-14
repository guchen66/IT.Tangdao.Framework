using System;

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