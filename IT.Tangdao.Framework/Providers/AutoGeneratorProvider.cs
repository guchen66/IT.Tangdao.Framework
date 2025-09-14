using IT.Tangdao.Framework.DaoCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Providers
{
    public class AutoGeneratorProvider
    {
        private static AutoGenerator _jsonData;
        public static string FileName { get; set; }

        private static AutoGenerator GetJsonData()
        {
            if (_jsonData == null)
            {
                if (FileName == null)
                {
                    throw new FileNotFoundException();
                }

                _jsonData = JsonConvert.DeserializeObject<AutoGenerator>(FileName);
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