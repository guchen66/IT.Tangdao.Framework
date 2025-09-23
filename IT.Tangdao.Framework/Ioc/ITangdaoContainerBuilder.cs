using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 建造者：负责收集注册语句，最后产出只读容器。
    /// 自身不解析、不缓存实例。
    /// </summary>
    public interface ITangdaoContainerBuilder
    {
        /// <summary>
        /// 直接暴露容器，供扩展方法链式 Register。
        /// </summary>
        ITangdaoContainer Container { get; }

        /// <summary>
        /// 一次性依赖校验：遍历 Registry，抛早失败。
        /// </summary>
        void ValidateDependencies();

        /// <summary>
        /// 结束建造，返回只写容器（后续可再 BuildProvider）。
        /// </summary>
        ITangdaoContainer Build();
    }
}