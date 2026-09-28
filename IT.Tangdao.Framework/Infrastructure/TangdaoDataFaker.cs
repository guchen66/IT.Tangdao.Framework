using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Extensions;
using IT.Tangdao.Framework.Faker;
using IT.Tangdao.Framework.Pooling;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    /// <summary>
    /// 门面层默认组件的共享实例持有者。
    /// <para>
    /// 注册表与空值策略都是<b>无状态</b>的（状态全在传入的上下文里），因此可以安全共享；
    /// 放进非泛型类是为了避免 <c>TangdaoDataFaker&lt;T&gt;</c> 每封闭一个类型就重建一份注册表。
    /// </para>
    /// </summary>
    internal static class FakeDefaults
    {
        /// <summary>默认生成器注册表。</summary>
        public static readonly ITangdaoGeneratorRegistry Registry = new DefaultGeneratorRegistry();

        /// <summary>默认空值策略（按概率判 null）。</summary>
        public static readonly ITangdaoNullPolicy NullPolicy = new ProbabilityNullPolicy();
    }

    /// <summary>
    /// 数据自动生成器（门面）。
    /// <para>
    /// 【职责】只做三件事：并发调度造值、串行收尾主键、把结果交回调用方。
    /// 具体"某个类型该生成什么值"全部委托给
    /// <see cref="ITangdaoGeneratorRegistry"/> 中的生成器，
    /// "某个属性该走哪条规则"委托给 <c>FakeRuleChain</c>。
    /// </para>
    /// <para>
    /// 【对外契约保持不变】<c>Build(int)</c> 的签名与返回语义与旧版完全一致，
    /// 调用方无需改动代码。变化只在内部：不再有硬编码的类型分支字典，
    /// 不再有进程级自增计数器，属性 setter 缓存键补上了类型全名。
    /// </para>
    /// <para>
    /// 【可替换的三个扩展点】<see cref="Registry"/>、<see cref="NullPolicy"/>、<see cref="Options"/>
    /// 均可由调用方整体替换，从而在不改库源码的前提下改变生成行为。
    /// </para>
    /// </summary>
    /// <typeparam name="T">待生成的类型，需具备公开无参构造函数</typeparam>
    public class TangdaoDataFaker<T> where T : class, new()
    {
        #region 可替换的扩展点

        private static ITangdaoGeneratorRegistry _registry = FakeDefaults.Registry;
        private static ITangdaoNullPolicy _nullPolicy = FakeDefaults.NullPolicy;
        private static TangdaoFakerOptions _options = TangdaoFakerOptions.Default;

        /// <summary>
        /// 生成器注册表。替换后本类型后续生成立即生效；传入 null 则恢复默认。
        /// </summary>
        public static ITangdaoGeneratorRegistry Registry
        {
            get { return _registry; }
            set { _registry = value ?? FakeDefaults.Registry; }
        }

        /// <summary>
        /// 可空属性的空值策略。替换后立即生效；传入 null 则恢复默认。
        /// </summary>
        public static ITangdaoNullPolicy NullPolicy
        {
            get { return _nullPolicy; }
            set { _nullPolicy = value ?? FakeDefaults.NullPolicy; }
        }

        /// <summary>
        /// 全局配置。替换后立即生效；传入 null 则恢复默认。
        /// </summary>
        public static TangdaoFakerOptions Options
        {
            get { return _options; }
            set { _options = value ?? TangdaoFakerOptions.Default; }
        }

        #endregion

        /// <summary>
        /// 属性 setter 缓存。
        /// <para>
        /// 【缓存键必须含类型全名】旧实现只用 <c>property.Name</c> 作键，并且依赖
        /// "泛型类的静态字段按封闭类型各存一份"这一<b>隐式</b>机制来避免跨类型串味。
        /// 这种依赖非常脆弱：一旦缓存被提升到非泛型层就会立刻出错。
        /// 这里把 <c>typeof(T).FullName</c> 显式写进键，无论缓存放在哪一层都正确。
        /// </para>
        /// </summary>
        private static readonly ConcurrentDictionary<string, Action<T, object>> _cachePropertySetters =
            new ConcurrentDictionary<string, Action<T, object>>();

        /// <summary>
        /// 可写属性列表（已剔除标记了 <see cref="IgnoreAttribute"/> 的属性），延迟初始化。
        /// </summary>
        private static readonly Lazy<PropertyInfo[]> _cachedProperties = new Lazy<PropertyInfo[]>(() =>
            typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !p.IsDefined(typeof(IgnoreAttribute), false))
                .ToArray());

        /// <summary>
        /// 生成指定数量的随机数据实例。
        /// </summary>
        /// <param name="count">生成数量，非正数时返回空集合</param>
        /// <returns>生成的数据集合</returns>
        public List<T> Build(int count)
        {
            if (count <= 0)
            {
                return new List<T>();
            }

            TangdaoFakerOptions options = _options ?? TangdaoFakerOptions.Default;
            T[] array = new T[count];

            if (options.EnableParallel)
            {
                // 线程本地随机源：每个工作线程持有一个 Random，
                // 既避开 Random 的线程安全问题，也省去"每个对象 new 一个 Random"的开销
                Parallel.For(
                    0,
                    count,
                    () => CreateSeededRandom(),
                    (i, state, localRandom) =>
                    {
                        array[i] = CreateRandomInstance(options, localRandom);
                        return localRandom;
                    },
                    localRandom => { });
            }
            else
            {
                Random random = CreateSeededRandom();
                for (int i = 0; i < count; i++)
                {
                    array[i] = CreateRandomInstance(options, random);
                }
            }

            // 串行收尾：并发阶段主键只写了占位值，这里统一回填连续序号
            AssignAutoIncrementIds(array);

            return array.ToList();
        }

        /// <summary>
        /// 创建一个受当前线程随机源驱动的实例。
        /// </summary>
        /// <param name="options">本次生成使用的配置</param>
        /// <param name="random">本线程的随机源</param>
        private static T CreateRandomInstance(TangdaoFakerOptions options, Random random)
        {
            TangdaoGeneratorContext context = new TangdaoGeneratorContext(options, Registry, random);
            T instance = new T();

            PropertyInfo[] properties = _cachedProperties.Value;

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                Action<T, object> setter = GetOrCreateSetter(property);

                object value;
                try
                {
                    value = GeneratePropertyValue(property, context);
                }
                catch (Exception)
                {
                    // 单个属性上的配置错误不应中断整批数据生成
                    continue;
                }

                // 值类型属性无法接受 null，跳过赋值以保留其默认值
                if (value == null && property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                {
                    continue;
                }

                setter(instance, value);
            }

            return instance;
        }

        /// <summary>
        /// 计算单个属性的值：先由空值策略裁决可空属性，再交责任链。
        /// </summary>
        /// <param name="property">目标属性</param>
        /// <param name="context">生成上下文</param>
        private static object GeneratePropertyValue(PropertyInfo property, TangdaoGeneratorContext context)
        {
            // 只有可空类型（Nullable<T>）才参与空值裁决：
            // 引用类型的 null 语义由属性自身决定，不应被概率随机影响
            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                ITangdaoNullPolicy policy = NullPolicy;
                if (policy != null && policy.ShouldBeNull(property, context))
                {
                    return null;
                }
            }

            return FakeRuleChain.ResolvePropertyValue(property, context);
        }

        /// <summary>
        /// 串行回填自增主键：从 1 开始按数组顺序连续赋值。
        /// <para>
        /// 序号与线程调度彻底解耦——并发阶段各线程只管造出对象，
        /// 谁先谁后不影响最终 ID，也不再依赖任何进程级静态计数器。
        /// </para>
        /// </summary>
        /// <param name="array">已生成的对象数组</param>
        private static void AssignAutoIncrementIds(T[] array)
        {
            PropertyInfo idProperty = GetAutoIncrementProperty();

            if (idProperty == null || array.Length == 0)
            {
                return;
            }

            Action<T, object> setter = GetOrCreateSetter(idProperty);
            int nextId = 1;

            for (int i = 0; i < array.Length; i++)
            {
                T instance = array[i];
                if (instance == null)
                {
                    continue;
                }

                setter(instance, Convert.ChangeType(nextId, FakeRuleChain.GetEffectiveType(idProperty)));
                nextId++;
            }
        }

        /// <summary>
        /// 找出需要自增的主键属性，找不到返回 null。
        /// <para>
        /// 判断口径复用 <see cref="PrimaryKeyIncrementRule.IsAutoIncrementKey"/>，
        /// 保证"生成阶段谁是主键"与"收尾阶段给谁编号"两个判断不会出现分歧。
        /// </para>
        /// </summary>
        private static PropertyInfo GetAutoIncrementProperty()
        {
            PropertyInfo[] properties = _cachedProperties.Value;

            for (int i = 0; i < properties.Length; i++)
            {
                if (PrimaryKeyIncrementRule.IsAutoIncrementKey(properties[i]))
                {
                    return properties[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 取属性的赋值委托，带缓存。
        /// </summary>
        /// <param name="property">目标属性</param>
        private static Action<T, object> GetOrCreateSetter(PropertyInfo property)
        {
            string key = typeof(T).FullName + "." + property.Name;
            return _cachePropertySetters.GetOrAdd(key, k => BuildSetter(property));
        }

        /// <summary>
        /// 用表达式树编译属性赋值委托。
        /// <para>
        /// 相比 <c>PropertyInfo.SetValue</c>（每次调用都要装箱与反射查找），
        /// 编译后的委托接近直接赋值的开销；而生成十万条数据时，
        /// 每个属性省下的这点开销会被放大十万倍。
        /// </para>
        /// </summary>
        /// <param name="property">目标属性</param>
        private static Action<T, object> BuildSetter(PropertyInfo property)
        {
            // 无公开 setter 的属性（含只读属性）直接给一个空操作，避免调用方额外判空
            if (!property.CanWrite || property.GetSetMethod() == null)
            {
                return (instance, value) => { };
            }

            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "instance");
            ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

            MemberExpression propertyAccess = Expression.Property(instanceParameter, property);
            UnaryExpression convertedValue = Expression.Convert(valueParameter, property.PropertyType);
            BinaryExpression assign = Expression.Assign(propertyAccess, convertedValue);

            return Expression.Lambda<Action<T, object>>(assign, instanceParameter, valueParameter).Compile();
        }

        /// <summary>
        /// 创建一个带随机种子的随机源。
        /// <para>
        /// 不用 <c>new Random()</c> 无参构造：在 .NET Framework 与 .NET Core 2.x 上，
        /// 无参构造以系统时钟为种子，多线程同时构造极易拿到同一种子、产出相同的随机序列。
        /// </para>
        /// </summary>
        private static Random CreateSeededRandom()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }
    }
}