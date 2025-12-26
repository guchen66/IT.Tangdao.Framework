using IT.Tangdao.Framework.Abstractions.Results;
using System.Collections.Generic;
using System.Windows.Documents;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IJsonQueryable
    {
        ResponseResult SelectValue(string key);

        ResponseResult<List<dynamic>> SelectKeys();

        ResponseResult<List<dynamic>> SelectValues();

        ResponseResult<List<T>> SelectObjects<T>() where T : new();
    }
}