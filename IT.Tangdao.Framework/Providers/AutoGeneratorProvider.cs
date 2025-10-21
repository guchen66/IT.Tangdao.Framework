using IT.Tangdao.Framework.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Infrastructure;

namespace IT.Tangdao.Framework.Providers
{
    public class AutoGeneratorProvider
    {
        private static AutoGeneratorEntity _jsonData;
        public static string FileName { get; set; }

        private static AutoGeneratorEntity GetJsonData()
        {
            if (_jsonData == null)
            {
                if (FileName == null)
                {
                    throw new FileNotFoundException();
                }

                _jsonData = JsonConvert.DeserializeObject<AutoGeneratorEntity>(FileName);
            }

            return _jsonData;
        }

        public static bool IsGenerated
        {
            get => GetJsonData().IsAuto;
        }

        public static bool IsSeedData
        {
            get => GetJsonData().IsSeedData;
        }
    }
}