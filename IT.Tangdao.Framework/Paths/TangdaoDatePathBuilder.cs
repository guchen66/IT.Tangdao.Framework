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
    /// 用于构建基于日期的路径，格式为：基础路径/年/月/日
    /// </summary>
    public readonly struct TangdaoDatePathBuilder
    {
        /// <summary>
        /// 基础路径
        /// </summary>
        private readonly AbsolutePath _basePath;

        /// <summary>
        /// 内部构造函数，由TangdaoPath类的工厂方法调用
        /// </summary>
        /// <param name="basePath">初始基础路径</param>
        internal TangdaoDatePathBuilder(AbsolutePath basePath) => _basePath = basePath;

        /// <summary>
        /// 添加单个路径段到基础路径
        /// 可以在日期结构之前添加更多的路径段
        /// </summary>
        /// <param name="segment">要添加的路径段</param>
        /// <returns>更新后的日期路径构建器</returns>
        public TangdaoDatePathBuilder Combine(string segment)
            => new TangdaoDatePathBuilder(_basePath.Combine(segment));

        /// <summary>
        /// 一次添加多个路径段到基础路径
        /// 可以在日期结构之前添加更多的路径段
        /// </summary>
        /// <param name="segments">要添加的路径段数组</param>
        /// <returns>更新后的日期路径构建器</returns>
        public TangdaoDatePathBuilder Combine(params string[] segments)
        {
            var tmp = _basePath;
            foreach (var s in segments)
                tmp = tmp.Combine(s);
            return new TangdaoDatePathBuilder(tmp);
        }

        /// <summary>
        /// 一次添加多个路径段到基础路径，支持IEnumerable<string>类型
        /// 可以在日期结构之前添加更多的路径段
        /// </summary>
        /// <param name="segments">要添加的路径段集合</param>
        /// <returns>更新后的日期路径构建器</returns>
        public TangdaoDatePathBuilder Combine(IEnumerable<string> segments)
        {
            var tmp = _basePath;
            foreach (var s in segments)
                tmp = tmp.Combine(s);
            return new TangdaoDatePathBuilder(tmp);
        }

        /// <summary>
        /// 构建日期目录路径
        /// 格式：基础路径/年/月/日
        /// </summary>
        /// <returns>构建好的日期目录路径</returns>
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
        /// 格式：基础路径/年/月/日/文件名
        /// </summary>
        /// <param name="fileName">要创建的文件名，默认为null（使用路径的最后一段作为文件名）</param>
        /// <returns>构建好的日期文件路径</returns>
        public AbsolutePath BuildFile(string fileName = null)
        {
            var dateDir = BuildDirectory();
            return dateDir.CreateFile(fileName);
        }

        /// <summary>
        /// 指定自定义基础路径
        /// </summary>
        /// <param name="basePath">自定义基础路径</param>
        /// <returns>更新后的日期路径构建器</returns>
        public TangdaoDatePathBuilder WithBasePath(string basePath)
            => new TangdaoDatePathBuilder(new AbsolutePath(basePath));

        /// <summary>
        /// 基于解决方案目录构建日期路径
        /// </summary>
        /// <returns>更新后的日期路径构建器</returns>
        public TangdaoDatePathBuilder FromSolution()
            => new TangdaoDatePathBuilder(TangdaoPath.Instance.GetSolutionDirectory());
    }
}