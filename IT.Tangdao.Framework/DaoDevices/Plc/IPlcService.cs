using IT.Tangdao.Framework.DaoCommon;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoDevices.Plc
{
    public interface IPlcService
    {
        PlcOption Read();
    }

    public class PlcService : IPlcService
    {
        internal PlcOption PlcOption;

        public PlcService()
        {
            var register = TangdaoContext.GetContext<PlcOption>();
            PlcOption = register.Option.AsOrFail<PlcOption>();
        }

        public PlcOption Read()
        {
            var register = TangdaoContext.GetContext<PlcOption>();
            PlcOption plcOption = register.Option.AsOrFail<PlcOption>();
            return plcOption;
        }
    }
}