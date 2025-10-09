using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Paths
{
    /// <summary>
    /// 链式拼装器
    /// </summary>
    public readonly struct TangdaoPathBuilder
    {
        private readonly AbsolutePath _root;

        internal TangdaoPathBuilder(AbsolutePath root) => _root = root;

        // 单段拼装
        public TangdaoPathBuilder Combine(string segment)
            => new TangdaoPathBuilder(_root.Combine(segment));

        // 一次拼多个，减少中间实例
        public TangdaoPathBuilder Combine(params string[] segments)
        {
            var tmp = _root;
            foreach (var s in segments) tmp = tmp.Combine(s);
            return new TangdaoPathBuilder(tmp);
        }

        public AbsolutePath Build() => _root;
    }
}