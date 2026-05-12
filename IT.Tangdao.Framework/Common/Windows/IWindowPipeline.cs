using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Events;
using IT.Tangdao.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IT.Tangdao.Framework.Windows
{
    /// <summary>
    /// Window管道接口
    /// </summary>
    public interface IWindowPipeline
    {
        /// <summary>
        /// 管道守卫
        /// </summary>
        IWindowGuard Guard { get; set; }

        /// <summary>
        /// 配置窗口类型
        /// </summary>
        IWindowPipeline Configure<TWindow>() where TWindow : Window;

        /// <summary>
        /// 设置是否允许打开窗体（默认false）
        /// </summary>
        IWindowPipeline SetActive(bool allow);

        /// <summary>
        /// 配置登录成功后的回调
        /// </summary>
        IWindowPipeline OnSuccess(Action action);

        /// <summary>
        /// 配置登录失败/取消后的回调
        /// </summary>
        IWindowPipeline OnFailure(Action action);

        /// <summary>
        /// 设计VM对应上下文数据
        /// </summary>
        /// <param name="context"></param>
        void SetContext(GuardContext context);

        /// <summary>
        /// 打开的模态
        /// </summary>
        ShowMode ShowMode { get; set; }

        bool Execute();
    }

    public static class WindowPipelineExtension
    {
        //public static bool Execute(this IWindowPipeline windowPipeline)
        //{
        //    return true;
        //}
    }
}