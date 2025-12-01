using IT.Tangdao.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    /// <summary>
    /// 链式路径构建器
    /// 提供流畅的API用于构建文件路径
    /// </summary>
    public readonly struct TangdaoPathBuilder
    {
        /// <summary>
        /// 当前构建的根路径
        /// </summary>
        private readonly AbsolutePath _root;

        /// <summary>
        /// 内部构造函数，由TangdaoPath类的工厂方法调用
        /// </summary>
        /// <param name="root">初始根路径</param>
        internal TangdaoPathBuilder(AbsolutePath root) => _root = root;

        /// <summary>
        /// 添加单个路径段
        /// </summary>
        /// <param name="segment">要添加的路径段</param>
        /// <returns>更新后的路径构建器</returns>
        public TangdaoPathBuilder Combine(string segment)
            => new TangdaoPathBuilder(_root.Combine(segment));

        /// <summary>
        /// 一次添加多个路径段，减少中间实例创建
        /// </summary>
        /// <param name="segments">要添加的路径段数组</param>
        /// <returns>更新后的路径构建器</returns>
        public TangdaoPathBuilder Combine(params string[] segments)
        {
            var tmp = _root;
            foreach (var s in segments)
                tmp = tmp.Combine(s);
            return new TangdaoPathBuilder(tmp);
        }

        /// <summary>
        /// 一次添加多个路径段，支持IEnumerable<string>类型
        /// </summary>
        /// <param name="segments">要添加的路径段集合</param>
        /// <returns>更新后的路径构建器</returns>
        public TangdaoPathBuilder Combine(IEnumerable<string> segments)
        {
            var tmp = _root;
            foreach (var s in segments)
                tmp = tmp.Combine(s);
            return new TangdaoPathBuilder(tmp);
        }

        /// <summary>
        /// 构建最终的绝对路径
        /// </summary>
        /// <returns>构建好的绝对路径</returns>
        public AbsolutePath Build() => _root;

        /// <summary>
        /// 构建路径并创建目录
        /// 如果目录已存在，则返回现有目录路径
        /// </summary>
        /// <returns>创建的目录的绝对路径</returns>
        public AbsolutePath BuildDirectory()
        {
            return Build().CreateDirectory();
        }

        /// <summary>
        /// 构建路径并创建文件
        /// 如果文件已存在，则返回现有文件路径
        /// </summary>
        /// <param name="fileName">要创建的文件名，默认为null（使用路径的最后一段作为文件名）</param>
        /// <returns>创建的文件的绝对路径</returns>
        public AbsolutePath BuildFile(string fileName = null)
        {
            return Build().CreateFile(fileName);
        }

        /// <summary>
        /// 更改文件扩展名
        /// </summary>
        /// <param name="extension">新的文件扩展名</param>
        /// <returns>更新后的路径构建器</returns>
        public TangdaoPathBuilder WithExtension(string extension)
        {
            return new TangdaoPathBuilder(_root.WithExtension(extension));
        }

        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <returns>父目录的路径构建器</returns>
        public TangdaoPathBuilder Parent()
        {
            return new TangdaoPathBuilder(_root.Parent());
        }
    }
}