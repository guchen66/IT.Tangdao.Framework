using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentConfigQueryable : IContentQueryable
    {
        ReadResult SelectConfig(string section);

        ReadResult SelectConfig<T>(string section) where T : class, new();

        ReadResult SelectConfigByJsonConvert<T>(string section) where T : class, new();

        ReadResult SelectCustomConfig(string configName, string section);
    }
}