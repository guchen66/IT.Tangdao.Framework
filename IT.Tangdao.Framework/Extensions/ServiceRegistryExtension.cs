using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    internal static class ServiceRegistryExtensions
    {
        public static void ValidateDependencies(this IServiceRegistry registry)
        {
            var visitor = new DependencyValidationVisitor(registry);
            foreach (var entry in registry.GetAllEntries())
                visitor.Visit(entry);
        }
    }
}