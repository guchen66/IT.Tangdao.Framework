using IT.Tangdao.Framework.DaoAdmin.IServices;
using IT.Tangdao.Framework.DaoDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.Services
{
    public class AlarmReadService : IAlarmReadService
    {
        public readonly ITangdaoClient _client;
        public AlarmReadService(ITangdaoClient client)
        {

            _client = client;

        }
        public async Task<TangdaoResponse> ReadAlarm<TEntity>(TEntity entity, string alarmId)
        {
            await Task.Delay(1000);
            return  new TangdaoResponse(false, AlarmBackResult.Failt);
        }
    }
}
