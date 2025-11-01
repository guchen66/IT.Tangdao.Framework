using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Devices.Plc
{
    public interface IPlcService
    {
        PlcOption Read();
    }

    public class PlcService : IPlcService
    {
        internal PlcOption PlcOption;
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(PlcService));

        public PlcService()
        {
            string tangdapTag = "1";// CryptoHelper.Decrypt(nameof(DeviceContainer));
            var register = TangdaoContext.GetInstance<PlcOption>(tangdapTag);
            Logger.WriteLocal(register.Ip);
            // PlcOption = register.AsOrFail<PlcOption>();
        }

        public PlcOption Read()
        {
            PlcOption plcOption = PlcOption;
            return plcOption;
        }
    }
}