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
        ResponseResult SelectAppConfig(string section);

        ResponseResult SelectAppConfig<T>(string section) where T : class, new();

        ResponseResult SelectConfigByJsonConvert<T>(string section) where T : class, new();

        ResponseResult SelectCustomConfig(string configName, string section);
    }
}