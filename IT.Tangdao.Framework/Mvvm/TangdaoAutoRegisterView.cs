using IT.Tangdao.Framework.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Mvvm
{
    /// <summary>
    /// 自动注册所有视图
    /// </summary>
    internal sealed class TangdaoAutoRegisterView
    {
        public static void Register<T>(ITangdaoContainer tangdaoContainer)
        {
            // tangdaoContainer.AddTangdaoScoped<T>();
        }
    }
}