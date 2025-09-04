using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoParameters.EventArg
{
    /// <summary>
    /// 模型变更事件参数
    /// </summary>
    public class ModelChangedEventArgs<T> : EventArgs
    {
        public T OldModel { get; }
        public T NewModel { get; }

        public ModelChangedEventArgs(T oldModel, T newModel)
        {
            OldModel = oldModel;
            NewModel = newModel;
        }
    }
}