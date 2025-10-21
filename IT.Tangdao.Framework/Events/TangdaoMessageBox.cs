using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using IT.Tangdao.Framework.Commands;

namespace IT.Tangdao.Framework.Events
{
    public static class TangdaoMessageBox
    {
        // 默认 0 秒 → 走原生
        public static MessageBoxResult Show(
            string text,
            string caption = "",
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            int autoCloseSeconds = 0,
            MessageBoxResult defaultResult = MessageBoxResult.OK)
        {
            if (autoCloseSeconds <= 0)
                return System.Windows.MessageBox.Show(text, caption, button, icon);

            // 倒计时窗口
            return ShowInternal(text, caption, button, icon, autoCloseSeconds, defaultResult);
        }

        // 内部实现
        private static MessageBoxResult ShowInternal(
            string text, string caption, MessageBoxButton button,
            MessageBoxImage icon, int seconds, MessageBoxResult defaultResult)
        {
            var win = new Window
            {
                Title = caption,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };

            // 倒计时文本
            var tb = new TextBlock
            {
                Text = $"{text}\n\n({seconds}s)",
                Margin = new Thickness(20),
                TextAlignment = TextAlignment.Center
            };

            // 按钮区
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            MessageBoxResult result = defaultResult;

            void CloseWith(MessageBoxResult r) { result = r; win.DialogResult = true; }

            if (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel)
                buttonPanel.Children.Add(new Button { Content = "OK", Width = 80, Margin = new Thickness(5), IsDefault = true, Command = MinidaoCommand.Create(() => CloseWith(MessageBoxResult.OK)) });

            if (button == MessageBoxButton.OKCancel || button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel)
            {
                var cancelText = button == MessageBoxButton.YesNo ? "No" : "Cancel";
                buttonPanel.Children.Add(new Button { Content = cancelText, Width = 80, Margin = new Thickness(5), IsCancel = true, Command = new TangdaoCommand(() => CloseWith(MessageBoxResult.Cancel)) });
            }

            var panel = new StackPanel { Children = { tb, buttonPanel } };
            win.Content = panel;

            // 倒计时
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1), IsEnabled = true };
            timer.Tick += (sender, e) =>
            {
                seconds--;
                tb.Text = $"{text}\n\n({seconds}s)";
                if (seconds == 0)
                {
                    timer.Stop();
                    CloseWith(defaultResult);
                }
            };
            timer.Start();
            win.ShowDialog();
            return result;
        }
    }
}