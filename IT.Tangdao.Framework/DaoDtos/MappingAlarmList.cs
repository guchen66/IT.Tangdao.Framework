using IT.Tangdao.Framework.DaoDtos.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IT.Tangdao.Framework.DaoDtos
{
    public  class MappingAlarmList:List<AlarmNotice>
    {
        public void Add(string viewName,string viewModelName)
        {
            RemoveAll((AlarmNotice it)=>it.ViewName.Equals(viewName));
            Add(new AlarmNotice 
            {
                ViewName = viewName,
                ViewModelName = viewModelName
            });
        }

        public new void Clear()
        {
            RemoveAll((AlarmNotice it) => true);
        }
    }
}
