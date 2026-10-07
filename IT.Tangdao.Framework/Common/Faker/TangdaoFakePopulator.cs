using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Attributes;
using IT.Tangdao.Framework.Faker;
using IT.Tangdao.Framework.Infrastructure;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Common.Faker
{
    /// <summary>
    /// 默认非泛型批量造值器：把 <c>TangdaoDataFaker&lt;T&gt;</c> 的造值语义平移到运行时类型上。
    /// <para>
    /// 【职责边界】本类只负责"调度"——并发造实例、串行回填主键、打包成 <c>List&lt;T&gt;</c>；
    /// "属性该出什么值"仍全部交给 <see cref="FakeRuleChain.ResolvePropertyValue"/>
    /// 与 <see cref="ITangdaoNullPolicy"/>，"实例怎么创建"交给 <c>ObjectCreator</c>，
    /// 随机源复用 <c>RandomCompat</c>，不另起一套造值逻辑。
    /// </para>
    /// <para>
    /// 【与泛型门面的差异】泛型门面用表达式树 <c>new T()</c> 与 <c>Action&lt;T, object&gt;</c> 委托；
    /// 非泛型路径下类型只在运行时可知，因此实例化改走 <c>ObjectCreator.CreateInstance(Type)</c>，
    /// 属性赋值走 <c>Action&lt;object, object&gt;</c> 编译委托，二者均已缓存。
    /// </para>
    /// <para>
    /// 【缓存键必须含类型全名】与门面同样的理由：<c>PropertyInfo</c> 与 setter 缓存一旦脱离
    /// "泛型静态字段按封闭类型各存一份"的隐式隔离，就必须靠类型全名显式区分，否则同名属性会串味。
    /// </para>
    /// </summary>
    public class TangdaoFakePopulator : ITangdaoFakePopulator
    {
        /// <summary>可写属性列表缓存（已剔除标记了 <c>IgnoreAttribute</c> 的属性），键为类型全名。</summary>
        private static readonly ConcurrentDictionary<string, PropertyInfo[]> _cacheProperties =
            new ConcurrentDictionary<string, PropertyInfo[]>();

        /// <summary>属性赋值委托缓存，键为"类型全名.属性名"。</summary>
        private static readonly ConcurrentDictionary<string, Action<object, object>> _cacheSetters =
            new ConcurrentDictionary<string, Action<object, object>>();

        /// <summary>
        /// 为指定类型生成一批随机实例。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="count">生成数量，非正数时返回空集合</param>
        /// <param name="options">生成配置，为 null 时按库默认配置</param>
        /// <param name="registry">生成器注册表，为 null 时按库默认注册表</param>
        /// <param name="nullPolicy">可空属性的空值策略，为 null 时按库默认策略</param>
        /// <returns>长度为 count 的 <c>List&lt;type&gt;</c></returns>
        public IList Populate(Type type, int count, TangdaoFakerOptions options, ITangdaoGeneratorRegistry registry, ITangdaoNullPolicy nullPolicy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            if (count <= 0)
            {
                return list;
            }

            TangdaoFakerOptions effectiveOptions = options ?? TangdaoFakerOptions.Default;
            object[] array = new object[count];

            if (effectiveOptions.EnableParallel)
            {
                Parallel.For(0, count, i =>
                {
                    array[i] = CreateRandomInstance(type, effectiveOptions, registry, nullPolicy);
                });
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    array[i] = CreateRandomInstance(type, effectiveOptions, registry, nullPolicy);
                }
            }

            // 串行收尾：并发阶段主键只写了占位值，这里统一回填连续序号
            AssignAutoIncrementIds(type, array);

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        /// <summary>
        /// 创建一个由当前线程随机源驱动的实例。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="options">本次生成使用的配置</param>
        /// <param name="registry">生成器注册表</param>
        /// <param name="nullPolicy">空值策略</param>
        private static object CreateRandomInstance(Type type, TangdaoFakerOptions options, ITangdaoGeneratorRegistry registry, ITangdaoNullPolicy nullPolicy)
        {
            // 随机源复用 RandomCompat：其内部为线程本地实例，并行造值时各线程互不干扰
            TangdaoGeneratorContext context = new TangdaoGeneratorContext(options, registry, RandomCompat.Shared);
            object instance = ObjectCreator.CreateInstance(type);

            PropertyInfo[] properties = GetProperties(type);

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                object value;
                try
                {
                    value = GeneratePropertyValue(property, context, nullPolicy);
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

                GetOrCreateSetter(type, property)(instance, value);
            }

            return instance;
        }

        /// <summary>
        /// 计算单个属性的值：先由空值策略裁决可空属性，再交责任链。
        /// </summary>
        /// <param name="property">目标属性</param>
        /// <param name="context">生成上下文</param>
        /// <param name="nullPolicy">空值策略</param>
        private static object GeneratePropertyValue(PropertyInfo property, TangdaoGeneratorContext context, ITangdaoNullPolicy nullPolicy)
        {
            // 只有可空类型（Nullable<T>）才参与空值裁决：
            // 引用类型的 null 语义由属性自身决定，不应被概率随机影响
            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                if (nullPolicy != null && nullPolicy.ShouldBeNull(property, context))
                {
                    return null;
                }
            }

            return FakeRuleChain.ResolvePropertyValue(property, context);
        }

        /// <summary>
        /// 串行回填自增主键：从 1 开始按数组顺序连续赋值。
        /// <para>
        /// 与泛型门面的 <c>AssignAutoIncrementIds</c> 语义等价，区别只在类型来自运行时：
        /// 先反射找出自增主键属性，再用编译好的 <c>Action&lt;object, object&gt;</c> 委托逐条写入。
        /// 序号与线程调度彻底解耦，也不依赖任何进程级静态计数器。
        /// </para>
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="array">已生成的对象数组</param>
        private static void AssignAutoIncrementIds(Type type, object[] array)
        {
            if (array.Length == 0)
            {
                return;
            }

            PropertyInfo idProperty = GetAutoIncrementProperty(type);
            if (idProperty == null)
            {
                return;
            }

            Action<object, object> setter = GetOrCreateSetter(type, idProperty);
            int nextId = 1;

            for (int i = 0; i < array.Length; i++)
            {
                object instance = array[i];
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
        /// <param name="type">目标类型</param>
        private static PropertyInfo GetAutoIncrementProperty(Type type)
        {
            PropertyInfo[] properties = GetProperties(type);

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
        /// 取类型的可写属性集合，带缓存。
        /// <para>
        /// 过滤口径与泛型门面一致：公共实例属性，且未被 <c>IgnoreAttribute</c> 标注
        /// （<c>inherit</c> 传 false，与门面逐字对齐）。
        /// </para>
        /// </summary>
        /// <param name="type">目标类型</param>
        private static PropertyInfo[] GetProperties(Type type)
        {
            return _cacheProperties.GetOrAdd(type.FullName, key => type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !p.IsDefined(typeof(IgnoreAttribute), false))
                .ToArray());
        }

        /// <summary>
        /// 取属性的赋值委托，带缓存。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="property">目标属性</param>
        private static Action<object, object> GetOrCreateSetter(Type type, PropertyInfo property)
        {
            string key = type.FullName + "." + property.Name;
            return _cacheSetters.GetOrAdd(key, k => BuildSetter(property));
        }

        /// <summary>
        /// 用表达式树编译非泛型属性赋值委托。
        /// <para>
        /// 相比 <c>PropertyInfo.SetValue</c>（每次调用都要装箱与反射查找），编译后的委托接近直接赋值的开销。
        /// 属性可能声明在基类上，因此转换目标取 <c>DeclaringType</c>；<c>DeclaringType</c> 缺失时
        /// 退化为 <c>SetValue</c>，保证极端情况下仍然可赋值。
        /// </para>
        /// </summary>
        /// <param name="property">目标属性</param>
        private static Action<object, object> BuildSetter(PropertyInfo property)
        {
            Type declaringType = property.DeclaringType;

            // 无公开 setter 的属性（含只读属性）直接给一个空操作，避免调用方额外判空
            if (declaringType == null || !property.CanWrite || property.GetSetMethod() == null)
            {
                return (instance, value) => { };
            }

            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
            ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

            MemberExpression propertyAccess = Expression.Property(Expression.Convert(instanceParameter, declaringType), property);
            UnaryExpression convertedValue = Expression.Convert(valueParameter, property.PropertyType);
            BinaryExpression assign = Expression.Assign(propertyAccess, convertedValue);

            return Expression.Lambda<Action<object, object>>(assign, instanceParameter, valueParameter).Compile();
        }
    }
}
