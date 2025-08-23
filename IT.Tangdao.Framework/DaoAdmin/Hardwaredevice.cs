using IT.Tangdao.Framework.DaoAdmin.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin
{
    public class Hardwaredevice : IHardwaredevice
    {
        public int Id { get; set; }

        public string DeviceName { get; set; }

        public bool IsConn { get; set; }

        public async Task<ReadResult> Open()
        {
            await Task.Delay(1000);
            return ReadResult.Success();
        }

        public async Task<ReadResult> Close()
        {
            await Task.Delay(1000);
            return ReadResult.Success();
        }

        public async Task<ReadResult> Read()
        {
            await Task.Delay(1000);
            return ReadResult.Success();
        }
    }
}