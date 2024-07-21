using IT.Tangdao.Framework.DaoAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Extensions
{
    public class ScanneringExtension
    {
        public static void GetScanObject<TModel>(TModel model)
        {
            var types=model.GetType().Assembly.GetTypes().Where(x=>Attribute.IsDefined(x,typeof(ScanningAttribute)));
        }
    }
}
