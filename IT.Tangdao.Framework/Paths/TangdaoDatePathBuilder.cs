using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    /// <summary>
    /// 日期路径流畅构建器
    /// </summary>
    public readonly struct TangdaoDatePathBuilder
    {
        private readonly AbsolutePath _basePath;

        internal TangdaoDatePathBuilder(AbsolutePath basePath) => _basePath = basePath;

        /// <summary>
        /// 构建日期目录路径
        /// </summary>
        public AbsolutePath BuildDirectory()
        {
            DateTime now = DateTime.Now;
            return _basePath
                .Combine(now.Year.ToString())
                .Combine(now.Month.ToString("00"))
                .Combine(now.Day.ToString("00")).CreateDirectory();
        }

        /// <summary>
        /// 构建日期文件路径
        /// </summary>
        public AbsolutePath BuildFile(string fileName = null)
        {
            var dateDir = BuildDirectory();
            return dateDir.CreateFile(fileName);
        }

        /// <summary>
        /// 指定自定义基础路径
        /// </summary>
        public TangdaoDatePathBuilder WithBasePath(string basePath)
            => new TangdaoDatePathBuilder(new AbsolutePath(basePath));

        /// <summary>
        /// 基于解决方案目录
        /// </summary>
        public TangdaoDatePathBuilder FromSolution()
            => new TangdaoDatePathBuilder(TangdaoPath.Instance.GetSolutionDirectory());
    }
}