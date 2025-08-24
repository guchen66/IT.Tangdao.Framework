using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework
{
    public sealed class TangdaoTask
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public string Duration => _stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public TangdaoTask()
        {
            _stopwatch.Start(); // 构造函数开始计时
        }
    }
}