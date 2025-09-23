using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.DaoMvvm;

namespace IT.Tangdao.Framework.Selectors
{
    /// <summary>
    /// ,PresentationTraceSources.TraceLevel=High
    /// </summary>
    public static class ViewModelSelector
    {
        #region 附加属性

        // 1. 显式静态构造
        static ViewModelSelector()
        {
            // 2. 强制触摸字段，防止 JIT 死代码消除
            var dummy = AutoWireViewModelProperty;
        }

        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached(
                "AutoWireViewModel",
                typeof(bool),
                typeof(ViewModelSelector),
                new PropertyMetadata(false, OnChanged));

        public static bool GetAutoWireViewModel(DependencyObject obj)
            => (bool)obj.GetValue(AutoWireViewModelProperty);

        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
            => obj.SetValue(AutoWireViewModelProperty, value);

        #endregion 附加属性

        /* 约定：
         * Views/FooView     → ViewModels/FooViewModel
         * 命名空间无关，只要类名对得上。
         */

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d is FrameworkElement view)
                //  view.Initialized += OnInitialized;   // 比 Loaded 早

                Register(view);
        }

        private static void Register(FrameworkElement view)
        {
            //  var view = (FrameworkElement)sender;
            //view.Initialized -= OnInitialized;         // 只跑一次

            try
            {
                // 1. 切断继承链
                view.ClearValue(FrameworkElement.DataContextProperty);

                var name = view.GetType().FullName.Replace("View", "ViewModel");
                var viewModelType = Type.GetType(name);
                if (viewModelType != null)
                {
                    var viewModel = TangdaoApplication.Provider.GetService(viewModelType);
                    view.DataContext = viewModel;
                }
            }
            catch { /* 静默：让设计器或运行时都不会炸 */ }
        }
    }
}