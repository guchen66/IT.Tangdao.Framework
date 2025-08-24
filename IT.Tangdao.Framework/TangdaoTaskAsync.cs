using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoTaskAsync
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public string Duration => _stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public TangdaoTaskAsync()
        {
            _stopwatch.Start(); // 构造函数开始计时
        }

        // 可以添加一些辅助方法
        public void Delay(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }
    }
}