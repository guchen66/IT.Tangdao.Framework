using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Results;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentJsonQueryable : IContentQueryable
    {
        ResponseResult SelectKeys();

        ResponseResult SelectValue(string key);
    }
}