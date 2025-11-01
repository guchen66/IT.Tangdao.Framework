using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Devices.Plc;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.Helpers;

namespace IT.Tangdao.Framework.Devices
{
    public sealed class DeviceContainer// : IDeviceContainer
    {
        private DeviceContainer()
        {
        }

        public static DeviceContainer Default { get; set; } = new DeviceContainer();

        /// <summary>
        /// 注册单个PLC
        /// </summary>
        /// <param name="option"></param>
        public void Register(PlcOption option)
        {
        }

        public void Register(Action<PlcOption> callback)
        {
            PlcOption option = new PlcOption();
            callback.Invoke(option);
            RegisterContext registerContext = new RegisterContext();
            registerContext.Option = option;
            string tangdapTag = "1";// CryptoHelper.Encrypt(nameof(DeviceContainer));
            TangdaoContext.SetInstance(tangdapTag, option);     //这里直接把对象传递过去使用Instance传递
        }

        /// <summary>
        /// 注册多个PLC
        /// </summary>
        /// <param name="option"></param>
        //public void Register(List<PlcOption> option)
        //{
        //}
    }
}