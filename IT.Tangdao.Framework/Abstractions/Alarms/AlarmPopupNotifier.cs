using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IT.Tangdao.Framework.DaoTasks;

namespace IT.Tangdao.Framework.Abstractions.Alarms
{
    internal sealed class AlarmPopupNotifier : IAlarmNotifier
    {
        public string Name => "Popup";

        public void OnNext(AlarmMessage alarm)
        {
            // 这里弹窗、写日志、播声音都可

            TangdaoTaskScheduler.ExecuteAsync(daoTaskAsync =>
            {
                MessageBox.Show(
                       alarm.Name,
                       alarm.IsCritical ? "严重报警" : "一般报警",
                       MessageBoxButton.OK,
                       alarm.IsCritical ? MessageBoxImage.Error : MessageBoxImage.Information);
            });
        }

        public void OnCompleted()
        { /* 释放资源 */ }

        public void OnError(Exception error)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}