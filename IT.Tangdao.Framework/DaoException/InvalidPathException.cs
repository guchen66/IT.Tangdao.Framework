using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoException
{
    /// <summary>
    /// 给定路径既不是文件也不是目录。
    /// </summary>
    public class InvalidPathException : TangdaoException
    {
        public string Path { get; }

        public InvalidPathException(string path) : base($"路径无效：{path}")
        {
            Path = path;
        }

        public InvalidPathException(string path, Exception inner) : base($"路径无效：{path}", inner)
        {
            Path = path;
        }
    }
}