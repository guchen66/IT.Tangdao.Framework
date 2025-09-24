using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    public interface ITangdaoModule
    {
        /// 注册服务（必须）
        void RegisterServices(ITangdaoContainer container);

        /// 模块初始化（可选）：此时 IOC 已 Build，可解析服务
        void OnInitialized(ITangdaoProvider provider);

        /// 模块优先级：数字越小越先注册；默认 0
        int Order { get; }

        /// 是否懒加载：true = 第一次用到该模块里的服务时才 RegisterServices
        bool Lazy { get; }
    }
}