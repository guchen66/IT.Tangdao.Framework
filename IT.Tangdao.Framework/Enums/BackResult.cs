using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Enums
{
    public enum BackResult
    {
        None = 0,
        Success = 1,        // 执行成功
        Cancel = 2,        // 用户取消
        Error = 3,          // 执行出错
        Retry = 4,          // 需要重试
    }
}