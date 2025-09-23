using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Ioc
{
    /// <summary>
    /// 作用域：在同一作用域内获取的服务实例始终相同。
    /// 支持嵌套，Dispose 时释放所有跟踪的实例。
    /// </summary>
    public interface ITangdaoScope : IDisposable
    {
        /// <summary>
        /// 当前作用域的解析器；生命周期策略优先从这里取缓存。
        /// </summary>
        ITangdaoProvider ScopedProvider { get; }

        /// <summary>
        /// 跟踪需要 Dispose 的实例（仅 Singleton 不用跟踪）。
        /// </summary>
        void TrackForDispose(object instance);
    }
}