using IT.Tangdao.Framework.DaoComponents;
using IT.Tangdao.Framework.DaoDtos.Options;
using IT.Tangdao.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoProvider: ITangdaoProvider
    {
        private ITangdaoContainerBuilder _tangdaoContainerBuilder;

        private TangdaoProviderOptions _options;

        public DaoComponentContext Context { get; set; }

        public TangdaoProvider(ITangdaoContainerBuilder tangdaoContainerBuilder, TangdaoProviderOptions options)
        {
            _tangdaoContainerBuilder = tangdaoContainerBuilder;
            _options = options;
        }

        public object Resolve(Type type)
        {
           return Resolve(TangdaoScope.FromContainerType(type), Context);
        }

        public object Resolve(object obj, DaoComponentContext context)
        {
            return obj;
        }

    }
}
