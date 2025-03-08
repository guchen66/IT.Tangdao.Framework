using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoHardwares
{
    public class HardwareService
    {
        private static HardwareService instance;
        private HardwareService()
        {
            //防止外部私有化
        }
        public static HardwareService Instance => instance ?? (instance=new HardwareService());
    }
}
