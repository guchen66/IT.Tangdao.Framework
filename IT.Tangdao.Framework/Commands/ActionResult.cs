using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Commands
{
    /// <summary>
    /// 用于在命令执行过程中传递操作状态、名称和参数信息
    /// 作为ActionTable中带参数委托的参数类型
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// 获取或设置操作的执行状态
        /// </summary>
        public ActionStatus Result { get; set; }

        /// <summary>
        /// 获取或设置操作的名称标识
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置操作的参数对象
        /// </summary>
        /// <remarks>
        /// 用于传递操作所需的输入参数
        /// </remarks>
        public ITangdaoParameter Parameter { get; set; }

        /// <summary>
        /// 创建一个表示成功状态的ActionResult实例
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="parameter">操作参数</param>
        /// <returns>成功状态的ActionResult实例</returns>
        public static ActionResult Success(string name = null, ITangdaoParameter parameter = null)
        {
            return new ActionResult
            {
                Result = ActionStatus.Success,
                Name = name,
                Parameter = parameter
            };
        }

        /// <summary>
        /// 创建一个表示取消状态的ActionResult实例
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="parameter">操作参数</param>
        /// <returns>取消状态的ActionResult实例</returns>
        public static ActionResult Cancel(string name = null, ITangdaoParameter parameter = null)
        {
            return new ActionResult
            {
                Result = ActionStatus.Cancel,
                Name = name,
                Parameter = parameter
            };
        }

        /// <summary>
        /// 创建一个表示错误状态的ActionResult实例
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="parameter">操作参数</param>
        /// <returns>错误状态的ActionResult实例</returns>
        public static ActionResult Error(string name = null, ITangdaoParameter parameter = null)
        {
            return new ActionResult
            {
                Result = ActionStatus.Error,
                Name = name,
                Parameter = parameter
            };
        }

        /// <summary>
        /// 创建一个表示默认状态的ActionResult实例
        /// </summary>
        /// <param name="name">操作名称</param>
        /// <param name="parameter">操作参数</param>
        /// <returns>默认状态的ActionResult实例</returns>
        public static ActionResult None(string name = null, ITangdaoParameter parameter = null)
        {
            return new ActionResult
            {
                Result = ActionStatus.None,
                Name = name,
                Parameter = parameter
            };
        }
    }
}