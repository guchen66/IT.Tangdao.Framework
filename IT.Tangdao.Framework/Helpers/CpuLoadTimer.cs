using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    /// <summary>
    /// CPU 负载检测 helper
    /// </summary>
    public static class CpuLoadTimer
    {
        private static readonly PerformanceCounter _counter =
            new PerformanceCounter("Processor", "% Processor Time", "_Total");

        public static float Current => _counter.NextValue();

        public static bool ShouldSkip(float threshold = 70f) => Current > threshold;
    }
}