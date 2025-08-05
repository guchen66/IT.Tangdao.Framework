using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    public interface INavigateService<T>
    {
        T Default { get; set; }

        void OnNavigatedTo(string viewModelName, ITangdaoParameter tangdaoParameter);
    }
}