using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.DaoDtos.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class TangdaoContainerBuilderExtension
    {
        public static TangdaoProvider Builder(this ITangdaoContainer container)
        {
            ITangdaoAdapter tangdaoAdapter = (ITangdaoAdapter)container;
           
            return new TangdaoProvider(tangdaoAdapter);
        }

        public static TangdaoProvider Builder(this TangdaoContainer container, ITangdaoAdapter tangdaoAdapter)
        {

            return new TangdaoProvider(tangdaoAdapter);
        }
        /*  public static TangdaoProvider Builder(this ITangdaoContainer container, bool isFromContainer)
          {
              return container.Builder(new TangdaoProviderOptions { IsFromContainer = isFromContainer });
          }

          public static TangdaoProvider Builder(this ITangdaoContainer container, TangdaoProviderOptions options)
          {
              if (container is null)
              {
                  throw new ArgumentNullException(nameof(container));
              }
              if (options is null)
              {
                  throw new ArgumentNullException(nameof(options));
              }

              return new TangdaoProvider(container, options);
          }*/
    }
}
