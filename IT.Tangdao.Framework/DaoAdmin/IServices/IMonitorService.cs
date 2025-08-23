using IT.Tangdao.Framework.DaoDtos.Options;
using IT.Tangdao.Framework.DaoEnums;
using IT.Tangdao.Framework.DaoEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoAdmin.IServices
{
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