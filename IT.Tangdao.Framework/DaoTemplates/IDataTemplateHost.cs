using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.DaoTemplates
{
    public interface IDataTemplateHost
    {
        TangdaoDataTemplates TangdaoDataTemplates { get; }

        bool IsDataTemplatesInitialized { get; }
    }
}
