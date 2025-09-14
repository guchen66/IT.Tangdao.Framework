using IT.Tangdao.Framework.Configurations;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.DaoEvents;
using IT.Tangdao.Framework.Parameters.EventArg;
using System;

namespace IT.Tangdao.Framework.Abstractions.IServices
{
    /// <summary>
    /// 本地文件（增删改）监控接口
    /// </summary>
    public interface IMonitorService
    {
        event EventHandler<DaoFileChangedEventArgs> FileChanged;

        /// <summary>
        /// 开始监控（使用默认配置）
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// 开始监控（使用自定义配置）
        /// </summary>
        void StartMonitoring(FileMonitorConfig config);

        /// <summary>
        /// 停止监控
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// 获取当前监控状态
        /// </summary>
        DaoMonitorStatus GetStatus();
    }
}