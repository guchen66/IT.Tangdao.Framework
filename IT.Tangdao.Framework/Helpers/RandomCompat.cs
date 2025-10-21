using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 等价于Random.Shared.Next(...)
    /// </summary>
    public static class RandomCompat
    {
        // 线程安全的单例 Random
        private static readonly Random _global = new Random();

        // 线程本地实例，解决 Random 并发问题
        private static readonly ThreadLocal<Random> _local =
            new ThreadLocal<Random>(() =>
            {
                // 在锁里播种，避免相同种子
                lock (_global) return new Random(_global.Next());
            });

        /// <summary>
        /// 等价于 .NET 6 的 Random.Shared
        /// </summary>
        public static Random Shared => _local.Value;

        /// <summary>
        /// 50 % 概率返回 true
        /// </summary>
        public static bool NextBool() => Shared.Next(2) == 0;

        /// <summary>
        /// 指定概率返回 true
        /// </summary>
        /// <param name="trueProbability">0.0 ~ 1.0</param>
        public static bool NextBool(double trueProbability)
        {
            if (trueProbability < 0.0 || trueProbability > 1.0)
                throw new ArgumentOutOfRangeException(nameof(trueProbability));
            return Shared.NextDouble() < trueProbability;
        }
    }
}