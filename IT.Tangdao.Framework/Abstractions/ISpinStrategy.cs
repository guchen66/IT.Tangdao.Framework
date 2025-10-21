using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions
{
    /// <summary>
    /// 插拔式自适应 SpinWait 策略包：PAUSE / HTAware / 指数退避
    /// </summary>
    public interface ISpinStrategy
    {
        /// <summary>执行一次自旋迭代</summary>
        void Spin();

        /// <summary>重置状态（新一次等待前调用）</summary>
        void Reset();
    }

    #region --------- 策略实现 ---------

    /// <summary>经典指数退避 + Yield</summary>
    public sealed class ExponentialBackoffStrategy : ISpinStrategy
    {
        private int _counter;

        public void Reset()
        { _counter = 0; }

        public void Spin()
        {
            int c = _counter;
            if (c < 10)
            {
                // 纯自旋
                for (int i = 0; i < (1 << c); i++) Thread.SpinWait(4);
            }
            else if (c < 20)
            {
                Thread.Yield();
            }
            else
            {
                Thread.Sleep(0);
            }
            _counter++;
        }
    }

    /// <summary>PAUSE 指令（x86/x64）+ HT 感知</summary>
    public sealed class PauseAwareStrategy : ISpinStrategy
    {
        private int _spin;

        public void Reset()
        { _spin = 0; }

        public void Spin()
        {
            // 前 2048 次使用 PAUSE
            if (_spin < 2048)
            {
                Thread.SpinWait(4);
                CpuPause();
            }
            else if (_spin < 4096)
            {
                Thread.Yield();
            }
            else
            {
                Thread.Sleep(1);
            }
            _spin++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CpuPause()
        {
            Thread.SpinWait(1);
        }
    }

    /// <summary>CPU 负载感知：高负载时立即让出</summary>
    public sealed class CpuLoadAwareStrategy : ISpinStrategy
    {
        private static readonly PerformanceCounter CpuCounter =
            new PerformanceCounter("Processor", "% Processor Time", "_Total");

        private int _spin;

        public void Reset()
        { _spin = 0; }

        public void Spin()
        {
            // 高负载立即 Yield
            if (CpuCounter.NextValue() > 70f)
            {
                Thread.Yield(); return;
            }
            // 低负载用 PAUSE
            if (_spin < 1024)
            {
                CpuPause();
            }
            else
            {
                Thread.Sleep(0);
            }
            _spin++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CpuPause()
        {
            // 同 PauseAwareStrategy
            if (IntPtr.Size == 4 || IntPtr.Size == 8)
                Thread.SpinWait(10);
        }
    }

    #endregion --------- 策略实现 ---------

    /// <summary>
    /// 自适应 SpinWait 外壳：可插拔策略
    /// </summary>
    public struct AdaptiveSpinWait
    {
        private ISpinStrategy _strategy;

        public AdaptiveSpinWait(ISpinStrategy strategy = null)
        {
            _strategy = strategy ?? new PauseAwareStrategy();
            _strategy.Reset();
        }

        public void Spin() => _strategy.Spin();

        public void Reset() => _strategy.Reset();

        /// <summary>快速旋转指定次数</summary>
        public void Spin(int iterations)
        {
            for (int i = 0; i < iterations; i++) Spin();
        }
    }

    /// <summary>
    /// 策略工厂
    /// </summary>
    public static class SpinStrategyFactory
    {
        public static ISpinStrategy ExponentialBackoff() => new ExponentialBackoffStrategy();

        public static ISpinStrategy PauseAware() => new PauseAwareStrategy();

        public static ISpinStrategy CpuLoadAware() => new CpuLoadAwareStrategy();
    }
}