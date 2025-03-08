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

        public async Task<IReadResult> Open()
        {
            IReadResult readResult = new IReadResult();
            await Task.Delay(1000);
            return readResult;
        }

        public async Task<IReadResult> Close()
        {
            IReadResult readResult = new IReadResult();
            await Task.Delay(1000);
            return readResult;
        }

        public async Task<IReadResult> Read()
        {
            IReadResult readResult = new IReadResult();
            await Task.Delay(1000);
            return readResult;
        }
    }
}