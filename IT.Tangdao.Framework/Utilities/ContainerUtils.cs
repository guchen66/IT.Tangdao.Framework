using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Utilities
{
    internal static class ContainerUtils
    {
        static ContainerUtils()
        {
            _builtCallbacks = new List<Action<ITangdaoProvider>>();
        }

        public static List<Action<ITangdaoProvider>> _builtCallbacks;

        public static void AddBuiltCallback(Action<ITangdaoProvider> callback)
        {
            _builtCallbacks.Add(callback);
        }

        public static void RaiseBuilt(ITangdaoProvider provider)
        {
            var exceptions = new List<Exception>();
            foreach (var back in _builtCallbacks)
            {
                try
                {
                    back.Invoke(provider);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException("模块初始化失败，见内部异常", exceptions);
        }

        public static void ValidateDependencies(ITangdaoContainer container)
        {
            container.Registry.ValidateDependencies();
        }
    }
}