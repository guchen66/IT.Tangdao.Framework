using IT.Tangdao.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    public interface ITangdaoRequest
    {
        T GetConfig<T>(string filePath = null) where T : class, new();

        T GetConfig<T>(string filePath, Action<T> onLoaded) where T : class, new();

        void ReloadConfig<T>(string filePath = null);
    }
}