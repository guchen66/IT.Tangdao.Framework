using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Configurations;
using System;

namespace IT.Tangdao.Framework.Extensions
{
    public static class PlcBuilderExtension
    {
        public static IPlcBuilder RegisterPlcOption(this IPlcBuilder builder, Action<PlcOption> option)
        {
            builder.Container.Configure(option);
            return builder;
        }
    }
}