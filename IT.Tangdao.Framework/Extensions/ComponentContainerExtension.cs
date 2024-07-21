using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public static class ComponentContainerExtension
    {
        public static TContainer RegisterContainer<TContainer>(this TContainer container, object options = default) where TContainer : class, new()
        {
            var type=container.GetType();
         //   return registry.RegisterComponent<TComponent, object>(options);

            return container;
        }
    }
}
