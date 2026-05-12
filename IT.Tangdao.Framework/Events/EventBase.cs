using IT.Tangdao.Framework.Enums;
using IT.Tangdao.Framework.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Events
{
    public abstract class EventBase
    {
    }

    public class WinEventBase : EventBase
    {
        public Type WindowType { get; set; }

        public ShowMode ShowMode { get; set; }

        public bool IsShow { get; set; }

        public GuardContext Context { get; set; }

        // 改为弱引用委托
        private WeakReference<Action> _successActionRef;

        private WeakReference<Action> _failureActionRef;

        public Action SucessAction
        {
            get => _successActionRef?.TryGetTarget(out var target) == true ? target : null;
            set => _successActionRef = value != null ? new WeakReference<Action>(value) : null;
        }

        public Action FailureAction
        {
            get => _failureActionRef?.TryGetTarget(out var target) == true ? target : null;
            set => _failureActionRef = value != null ? new WeakReference<Action>(value) : null;
        }

        public void InvokeSuccess()
        {
            if (SucessAction != null)
            {
                SucessAction();
                SucessAction = null;  // 执行后清理
            }
        }

        public void InvokeFailure()
        {
            if (FailureAction != null)
            {
                FailureAction();
                FailureAction = null;  // 执行后清理
            }
        }
    }
}