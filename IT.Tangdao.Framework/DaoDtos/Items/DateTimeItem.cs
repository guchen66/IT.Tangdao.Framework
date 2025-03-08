using IT.Tangdao.Framework.DaoMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IT.Tangdao.Framework.DaoDtos.Items
{
    public class DateTimeItem : DaoViewModelBase
    {
        private DateTime _currentDate;

        public DateTime CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        private DispatcherTimer timer;

        public DateTimeItem()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentDate = DateTime.Now;
        }

        private async void UpdateDateAsync()
        {
            while (true)
            {
                CurrentDate = DateTime.Now;
                await Task.Delay(TimeSpan.FromSeconds(1)); // 每秒更新一次
            }
        }
    }
}