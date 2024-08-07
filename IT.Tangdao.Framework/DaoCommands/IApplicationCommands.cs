using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoCommands
{
    /// <summary>
    /// 在接口中定义一些应用程序级别的命令，如保存日志、退出应用程序
    /// 将应用程序级别的命令统一放置到 IApplicationCommands 接口中，
    /// 可以将这些命令从具体的业务逻辑中解耦出来，
    /// 从而提高代码的可维护性和可测试性。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IApplicationCommands<T>
    {
        MinidaoCommand<T> MinidaoCommand { get; set; }
    }
}
