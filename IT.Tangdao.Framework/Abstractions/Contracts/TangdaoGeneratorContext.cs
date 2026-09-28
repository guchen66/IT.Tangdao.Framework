using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Contracts
{
    /// <summary>
    /// 生成上下文：在单次生成过程中传递随机源、配置、注册表与递归深度。
    /// <para>
    /// 为什么需要它：生成器与规则都要用随机数、要看配置、要查注册表，
    /// 如果各自 <c>new Random()</c> 或读静态字段，就会在并行场景下互相干扰。
    /// 把它们统一收进上下文本对象，由调用方（<c>Build</c>）按线程创建并下发，
    /// 随机源与状态就被天然隔离在各线程内部。
    /// </para>
    /// <para>
    /// 注意：本类<b>不持有可变共享状态</b>，是"只读行李"，可安全在同一次生成内传递。
    /// </para>
    /// </summary>
    public sealed class TangdaoGeneratorContext
    {
        /// <summary>生成配置。</summary>
        public TangdaoFakerOptions Options { get; private set; }

        /// <summary>生成器注册表。</summary>
        public ITangdaoGeneratorRegistry Registry { get; private set; }

        /// <summary>
        /// 随机源。
        /// <para>并行场景下每个线程各自持有一个实例，因此这里无需加锁。</para>
        /// </summary>
        public Random Random { get; private set; }

        /// <summary>当前递归深度，顶层为 0。</summary>
        public int Depth { get; private set; }

        /// <summary>
        /// 创建顶层上下文（深度 0）。
        /// </summary>
        /// <param name="options">生成配置</param>
        /// <param name="registry">生成器注册表</param>
        /// <param name="random">随机源</param>
        public TangdaoGeneratorContext(TangdaoFakerOptions options, ITangdaoGeneratorRegistry registry, Random random)
            : this(options, registry, random, 0)
        {
        }

        /// <summary>
        /// 创建指定深度的上下文。
        /// </summary>
        /// <param name="options">生成配置</param>
        /// <param name="registry">生成器注册表</param>
        /// <param name="random">随机源</param>
        /// <param name="depth">当前递归深度</param>
        public TangdaoGeneratorContext(TangdaoFakerOptions options, ITangdaoGeneratorRegistry registry, Random random, int depth)
        {
            Options = options;
            Registry = registry;
            Random = random;
            Depth = depth;
        }

        /// <summary>
        /// 派生子上下文，深度加一。
        /// <para>用于嵌套对象递归：只递增深度，复用同一随机源（同一线程内无需新建）。</para>
        /// </summary>
        public TangdaoGeneratorContext CreateChild()
        {
            return new TangdaoGeneratorContext(Options, Registry, Random, Depth + 1);
        }

        /// <summary>
        /// 返回 [min, max) 区间内的随机整数。
        /// <para>内部做了边界保护：min &gt; max 时自动交换；min == max 时直接返回 min，
        /// 避免调用方传入非法区间导致抛异常。</para>
        /// </summary>
        /// <param name="min">下界（含）</param>
        /// <param name="max">上界（不含）</param>
        public int Next(int min, int max)
        {
            if (min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }

            if (min == max)
            {
                return min;
            }

            return Random.Next(min, max);
        }

        /// <summary>
        /// 返回 [0,1) 区间内的随机小数。
        /// </summary>
        public double NextDouble()
        {
            return Random.NextDouble();
        }
    }
}
