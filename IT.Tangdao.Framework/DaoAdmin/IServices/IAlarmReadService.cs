using IT.Tangdao.Framework.DaoDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
    public interface IAlarmReadService
    {
        Task<TangdaoResponse> ReadAlarm<TEntity>(TEntity entity, string alarmId);
    }
}
