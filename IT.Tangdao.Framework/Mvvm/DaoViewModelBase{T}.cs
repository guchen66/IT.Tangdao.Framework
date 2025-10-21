using IT.Tangdao.Framework.Commands;
using IT.Tangdao.Framework.Common;
using IT.Tangdao.Framework.EventArg;
using IT.Tangdao.Framework.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IT.Tangdao.Framework.Mvvm
{
    /// <summary>
    /// 带 Model 的 ViewModel 基类。
    /// T 为业务实体（Model），默认实现深拷贝/验证/异步初始化/撤销/繁忙标记。
    /// </summary>
    public abstract class DaoViewModelBase<T> : DaoViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        // 验证规则字典
        private readonly Dictionary<string, Func<T, string>> _validationRules = new Dictionary<string, Func<T, string>>();

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private T _model;

        public T Model
        {
            get => _model;
            set
            {
                var oldValue = _model;
                SetProperty(ref _model, value, () => OnModelChanged(oldValue, value));
            }
        }

        /// <summary>
        /// 模型变更时触发，提供旧值和新值
        /// </summary>
        /// <param name="oldModel">旧的模型值</param>
        /// <param name="newModel">新的模型值</param>
        protected virtual void OnModelChanged(T oldModel, T newModel)
        {
            // 1. 触发模型变更事件（带参数）
            ModelChanged?.Invoke(this, new ModelChangedEventArgs<T>(oldModel, newModel));

            // 2. 重新验证所有属性
            Validate();

            // 3. 通知所有属性可能已变更
            //  RaiseAllPropertiesChanged();

            // 4. 执行派生类的自定义逻辑
            OnModelChangedCore(oldModel, newModel);
        }

        /// <summary>
        /// 供派生类重写的模型变更处理方法
        /// </summary>
        protected virtual void OnModelChangedCore(T oldModel, T newModel)
        {
            // 派生类可以重写此方法添加自定义逻辑
        }

        /// <summary>
        /// 模型变更事件（带参数）
        /// </summary>
        public event EventHandler<ModelChangedEventArgs<T>> ModelChanged;

        // 添加验证规则
        public void AddValidationRule(string propertyName, Func<T, string> validationRule)
        {
            _validationRules[propertyName] = validationRule;
        }

        // 验证整个模型
        public bool Validate()
        {
            ClearErrors();
            bool isValid = true;

            foreach (var rule in _validationRules)
            {
                var error = rule.Value(Model);
                if (!string.IsNullOrEmpty(error))
                {
                    AddError(rule.Key, error);
                    isValid = false;
                }
            }

            return isValid;
        }

        // 验证单个属性
        public bool ValidateProperty(string propertyName)
        {
            if (_validationRules.TryGetValue(propertyName, out var rule))
            {
                var error = rule(Model);
                if (!string.IsNullOrEmpty(error))
                {
                    AddError(propertyName, error);
                    return false;
                }
                RemoveError(propertyName);
            }
            return true;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors()
        {
            var propertyNames = _errors.Keys.ToList();
            _errors.Clear();
            foreach (var propertyName in propertyNames)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            _errors.TryGetValue(propertyName, out var errors);
            return errors;
        }
    }
}