using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Results;
using IT.Tangdao.Framework.Configurations;

namespace IT.Tangdao.Framework.Abstractions.FileAccessor
{
    public interface IContentIniQueryable
    {
        ResponseResult<IniConfig> SelectIni(string section);
    }
}