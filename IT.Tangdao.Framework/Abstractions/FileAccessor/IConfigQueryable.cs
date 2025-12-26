using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IConfigQueryable
    {
        ResponseResult<TangdaoSortedDictionary<string, string>> SelectAppConfig(string section);

        ResponseResult SelectAppConfig<T>(string section) where T : class, new();

        ResponseResult SelectConfigByJsonConvert<T>(string section) where T : class, new();

        ResponseResult<Dictionary<string, string>> SelectCustomConfig(string configName, string section);
    }
}