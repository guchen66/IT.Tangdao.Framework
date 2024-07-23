using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public interface ITangdaoContainer: ITangdaoContainerBuilder
    {
        /// <summary>
        /// 注册无参数类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        void RegisterScoped<TType>();

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        void RegisterScoped<TType>(params object[] obj);

        /// <summary>
        /// 注册类型和实现类
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TypeImple"></typeparam>
        void RegisterScoped<TType,TypeImple>() where TypeImple:TType;

    }
}
