
using IT.Tangdao.Framework.Abstractions.Contracts;
using IT.Tangdao.Framework.Faker;
using IT.Tangdao.Framework.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IT.Tangdao.Framework.Faker
{
    /// <summary>
    /// 默认注册表：登记库内置的全部生成器与命名模板，同时开放扩展。
    /// <para>
    /// 【生成器的匹配方式】一个生成器可能负责多个类型（例如 <c>IntegerValueGenerator</c> 负责八种整数），
    /// 因此无法用 <c>Dictionary&lt;Type, Generator&gt;</c> 精确索引，只能按<b>注册顺序</b>遍历、
    /// 由生成器自述 <c>CanGenerate</c>。这带来一条重要规则：
    /// <b>越靠前的生成器优先级越高，兜底的 <c>NestedObjectValueGenerator</c> 必须最后注册</b>，
    /// 否则它会把所有类型都截胡。
    /// </para>
    /// <para>
    /// 【后注册者优先】<c>Register</c> 采用头插：调用方注册的生成器排在内置生成器之前，
    /// 因此可以用自定义实现覆盖库的默认行为（例如让 <c>string</c> 产出中文姓名）。
    /// </para>
    /// <para>
    /// 【并发】生成器数组使用"写时复制 + volatile 读"，读路径无锁，
    /// 适配 <c>Build</c> 的并行生成；模板表用 <c>ConcurrentDictionary</c>。
    /// </para>
    /// </summary>
    public sealed class DefaultGeneratorRegistry : ITangdaoGeneratorRegistry
    {
        /// <summary>写锁，仅在注册时短暂持有。</summary>
        private readonly object _syncRoot = new object();

        /// <summary>生成器列表快照，读路径直接读该引用，不加锁。</summary>
        private volatile ITangdaoValueGenerator[] _generators;

        /// <summary>模板表，模板名不区分大小写。</summary>
        private readonly ConcurrentDictionary<string, Func<TangdaoGeneratorContext, object>> _templates =
            new ConcurrentDictionary<string, Func<TangdaoGeneratorContext, object>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 创建一个包含库内置生成器与模板的注册表。
        /// </summary>
        public DefaultGeneratorRegistry()
        {
            _generators = new ITangdaoValueGenerator[0];

            // 顺序即优先级，兜底的嵌套对象生成器必须放最后
            RegisterCore(new StringValueGenerator());
            RegisterCore(new IntegerValueGenerator());
            RegisterCore(new FloatingValueGenerator());
            RegisterCore(new BooleanValueGenerator());
            RegisterCore(new DateTimeValueGenerator());
            RegisterCore(new EnumValueGenerator());
            RegisterCore(new GuidValueGenerator());
            RegisterCore(new NestedObjectValueGenerator());

            RegisterDefaultTemplates();
        }

        /// <summary>
        /// 注册一个类型生成器，后注册者优先于先注册者。
        /// </summary>
        /// <param name="generator">生成器实例</param>
        public void Register(ITangdaoValueGenerator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            lock (_syncRoot)
            {
                ITangdaoValueGenerator[] current = _generators;
                ITangdaoValueGenerator[] next = new ITangdaoValueGenerator[current.Length + 1];

                // 新生成器插在 0 号位：用户扩展优先于库内置
                next[0] = generator;
                Array.Copy(current, 0, next, 1, current.Length);
                _generators = next;
            }
        }

        /// <summary>
        /// 注册一个命名模板，同名模板后者覆盖前者。
        /// </summary>
        /// <param name="template">模板名</param>
        /// <param name="factory">模板工厂</param>
        public void RegisterTemplate(string template, Func<TangdaoGeneratorContext, object> factory)
        {
            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentException("模板名不能为空", nameof(template));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _templates[template] = factory;
        }

        /// <summary>
        /// 按类型查找生成器，按注册顺序返回第一个认领该类型的生成器。
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="generator">命中的生成器</param>
        public bool TryResolve(Type type, out ITangdaoValueGenerator generator)
        {
            ITangdaoValueGenerator[] snapshot = _generators;

            for (int i = 0; i < snapshot.Length; i++)
            {
                if (snapshot[i].CanGenerate(type))
                {
                    generator = snapshot[i];
                    return true;
                }
            }

            generator = null;
            return false;
        }

        /// <summary>
        /// 按模板名查找模板工厂。
        /// </summary>
        /// <param name="template">模板名</param>
        /// <param name="factory">命中的工厂</param>
        public bool TryResolveTemplate(string template, out Func<TangdaoGeneratorContext, object> factory)
        {
            factory = null;

            if (string.IsNullOrEmpty(template))
            {
                return false;
            }

            return _templates.TryGetValue(template, out factory);
        }

        /// <summary>
        /// 内部注册：尾插，保持库内置生成器的既定优先级顺序。
        /// </summary>
        /// <param name="generator">生成器实例</param>
        private void RegisterCore(ITangdaoValueGenerator generator)
        {
            ITangdaoValueGenerator[] current = _generators;
            ITangdaoValueGenerator[] next = new ITangdaoValueGenerator[current.Length + 1];

            Array.Copy(current, 0, next, 0, current.Length);
            next[current.Length] = generator;
            _generators = next;
        }

        /// <summary>
        /// 登记六个内置命名模板。
        /// <para>
        /// 【修正说明】旧实现用 <c>switch</c> 分发模板，缺少 <c>Hobby</c> 分支，
        /// 导致 <c>Template = MockTemplate.Hobby</c> 静默退化成随机字符串。
        /// 改为注册表登记后，模板与实现一一对应，不再存在"漏写分支"的可能；
        /// 且 <see cref="MockTemplate"/> 里的每个常量都在此处有对应登记。
        /// </para>
        /// </summary>
        private void RegisterDefaultTemplates()
        {
            RegisterTemplate(MockTemplate.ChineseName, context => FakedataUtils.GetRandomChineseName(context.Random));
            RegisterTemplate(MockTemplate.Mobile, context => FakedataUtils.GenerateChineseMobileNumber(context.Random));
            RegisterTemplate(MockTemplate.City, context => FakedataUtils.GetRandomChineseCity(context.Random));
            RegisterTemplate(MockTemplate.Date, context => FakedataUtils.GenerateRandomDateTime(context.Random, context.Options.MinDate, context.Options.MaxDate));
            RegisterTemplate(MockTemplate.Email, context => FakedataUtils.CreateRandomEmail(context.Random));
            RegisterTemplate(MockTemplate.Hobby, context => FakedataUtils.GetRandomHobby(context.Random));
        }
    }
}
