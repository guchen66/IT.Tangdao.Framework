using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    /// <summary>
    /// 依赖注入的服务注册模式
    /// </summary>
    public enum RegisterMode
    {
        /// <summary>
        /// 瞬时模式（Transient）
        /// 每次从容器中解析（获取）服务时，都会创建一个全新的实例。
        /// 生命周期最短，无状态服务推荐使用此模式。
        /// 适用于：轻量级、无状态的服务，如工具类、仓储类。
        /// </summary>
        Transient,

        /// <summary>
        /// 单例模式（Singleton）
        /// 整个应用程序生命周期内，只创建一个实例，所有地方共享同一个引用。
        /// 生命周期最长，注意线程安全问题。
        /// 适用于：配置管理、日志记录、事件聚合器等全局唯一且可能持有状态的服务。
        /// </summary>
        Singleton,

        /// <summary>
        /// 作用域模式（Scoped）
        /// 在每个作用域（Scope）内创建唯一的实例。同一个作用域内解析多次得到同一个实例，不同作用域之间相互隔离。
        /// 生命周期：随作用域开始而创建，随作用域结束而释放。
        /// 适用于：Web 请求（每个 HTTP 请求一个作用域）、工作单元（Unit of Work）、或需要在一个独立操作单元内共享状态的服务。
        /// 示例：一个 HTTP 请求内多次获取 DbContext，得到的是同一个实例；不同请求之间互不影响。
        /// 注：在 WPF 等桌面应用中，如果没有手动创建作用域，其行为通常类似于 Transient。
        /// </summary>
        Scoped
    }
}