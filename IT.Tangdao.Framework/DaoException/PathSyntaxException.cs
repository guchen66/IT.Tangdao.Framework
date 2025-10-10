using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 对路径进行错误处理
    /// </summary>
    public sealed class PathSyntaxException : FormatException
    {
        public PathSyntaxException(string path, string reason)
            : base($"路径 \"{path}\" 格式错误：{reason}") { }
    }
}