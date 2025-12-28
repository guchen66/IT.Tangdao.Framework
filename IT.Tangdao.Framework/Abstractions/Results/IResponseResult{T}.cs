using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Results
{
    /// <summary>
    /// 泛型响应结果接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IResponseResult<out T> : IResponseResult
    {
        /// <summary>
        /// 响应到的数据
        /// </summary>
        T Data { get; }
    }
}