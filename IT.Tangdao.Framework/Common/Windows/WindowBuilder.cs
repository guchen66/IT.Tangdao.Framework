using IT.Tangdao.Framework.Abstractions.Loggers;
using IT.Tangdao.Framework.Ioc;
using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Windows
{
    /// <summary>
    /// 窗口构建器实现
    /// </summary>
    public class WindowBuilder : IWindowBuilder
    {
        private readonly List<IWindowPipeline> _pipelines = new List<IWindowPipeline>();

        public void UseGuard<TGuard>(Action<IWindowPipeline> configure) where TGuard : IWindowGuard
        {
            var guard = Activator.CreateInstance<TGuard>();
            var pipeline = ServiceLocator.Default.GetService<IWindowPipeline>();
            pipeline.Guard = guard;
            // 执行用户配置，收集参数
            configure(pipeline);

            _pipelines.Add(pipeline);
        }

        /// <summary>
        /// 框架内部调用：执行所有已配置的管道
        /// </summary>
        public bool ExecuteAll()
        {
            foreach (var pipeline in _pipelines)
            {
                if (!pipeline.Execute())
                {
                    // 任一管道失败，整体失败
                    return false;
                }
            }
            return true;
        }
    }
}