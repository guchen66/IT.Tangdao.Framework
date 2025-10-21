using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// 当判空 → 映射 → 过滤 → 默认值
    /// 或者想把“可能缺失”作为一等概念在 API 里传递时候使用
    /// 否则不建议使用
    /// </summary>
    public struct TangdaoOptional<T> : IEquatable<TangdaoOptional<T>>
    {
        // 真正的值；当 _hasValue==false 时它永远为 default(T)
        private readonly T _value;

        // 快速判断是否存在值，避免反复比较 null
        private readonly bool _hasValue;

        #region 构造与工厂

        /// <summary>
        /// 内部构造函数；由静态工厂调用。
        /// </summary>
        private TangdaoOptional(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        /// <summary>
        /// 创建一个包含值的实例。
        /// </summary>
        public static TangdaoOptional<T> Some(T value) =>
            new TangdaoOptional<T>(value, value != null);

        /// <summary>
        /// 创建一个空实例（None）。
        /// </summary>
        public static TangdaoOptional<T> None() =>
            default;

        #endregion 构造与工厂

        #region 核心属性

        /// <summary>
        /// 是否包含值（true=Some，false=None）。
        /// </summary>
        public bool HasValue => _hasValue;

        /// <summary>
        /// 直接取值；如果是 None 则抛 InvalidOperationException。
        /// 与 Nullable<T>.Value 行为一致。
        /// </summary>
        public T Value => _hasValue
            ? _value
            : throw new InvalidOperationException("Optional has no value.");

        #endregion 核心属性

        #region 常用转换

        /// <summary>
        /// 有值返回自己的值，否则返回指定默认值。
        /// 与 Nullable<T>.GetValueOrDefault 完全同义。
        /// </summary>
        public T ValueOrDefault(T fallback) => _hasValue ? _value : fallback;

        /// <summary>
        /// 有值返回自己的值，否则通过工厂委托现场生成默认值。
        /// 延迟计算，避免无谓开销。
        /// </summary>
        public T ValueOrElse(Func<T> fallbackFactory) =>
            _hasValue ? _value : fallbackFactory();

        #endregion 常用转换

        #region 函数式组合

        /// <summary>
        /// 如果满足条件则保持 Some，否则变成 None。
        /// </summary>
        public TangdaoOptional<T> Where(Func<T, bool> predicate) =>
            _hasValue && predicate(_value) ? this : default;

        /// <summary>
        /// 映射/投影：把当前值转换成另一种类型。
        /// 如果当前是 None，则直接返回目标类型的 None。
        /// </summary>
        public TangdaoOptional<TResult> Select<TResult>(Func<T, TResult> selector) =>
            _hasValue ? TangdaoOptional<TResult>.Some(selector(_value)) : default;

        /// <summary>
        /// 扁平映射：selector 本身也返回 Optional，避免嵌套。
        /// </summary>
        public TangdaoOptional<TResult> SelectMany<TResult>(Func<T, TangdaoOptional<TResult>> selector) =>
            _hasValue ? selector(_value) : default;

        #endregion 函数式组合

        #region 副作用

        /// <summary>
        /// 存在值时才执行委托；常用于触发副作用（日志、UI 更新等）。
        /// </summary>
        public void Invoke(Action<T> onSome)
        {
            if (_hasValue) onSome(_value);
        }

        /// <summary>
        /// 无论有没有值都执行对应委托；比 Invoke 更完整的模式匹配。
        /// </summary>
        public void Match(Action<T> onSome, Action onNone)
        {
            if (_hasValue) onSome(_value); else onNone();
        }

        #endregion 副作用

        #region 相等与哈希

        public bool Equals(TangdaoOptional<T> other) =>
            _hasValue == other._hasValue &&
            EqualityComparer<T>.Default.Equals(_value, other._value);

        public override bool Equals(object obj) =>
            obj is TangdaoOptional<T> o && Equals(o);

        public override int GetHashCode()
        {
            unchecked
            {
                return (_hasValue.GetHashCode() * 397) ^
                       EqualityComparer<T>.Default.GetHashCode(_value);
            }
        }

        public static bool operator ==(TangdaoOptional<T> left, TangdaoOptional<T> right) =>
            left.Equals(right);

        public static bool operator !=(TangdaoOptional<T> left, TangdaoOptional<T> right) =>
            !left.Equals(right);

        #endregion 相等与哈希
    }
}