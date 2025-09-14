using System;

namespace IT.Tangdao.Framework.Extensions
{
    public static class OptionsServerExtension
    {
        public static ITangdaoContainer Configure<TOptions>(this ITangdaoContainer container, Action<TOptions> options) where TOptions : class
        {
            container.Configure(options);
            return container;
        }
    }
}