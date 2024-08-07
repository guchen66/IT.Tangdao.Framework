using IT.Tangdao.Framework.DaoEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace IT.Tangdao.Framework.DaoCommon
{
    public class DaoToast
    {
        public event EventHandler<RoutedEventArgs> Loaded;

        public void DaoToast_Loaded(object sender, RoutedEventArgs e)
        {
           /* var osVersion = Environment.OSVersion;
            if (osVersion.Platform == PlatformID.Win32NT &&
                osVersion.Version.Major > 10 ||
                (osVersion.Version.Major == 10 && osVersion.Version.Build >= 15063))
            {
                global::WinRT.ComWrappersSupport.InitializeComWrappers();

                // 以下 XML 的构建，请看
                // https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/adaptive-interactive-toasts?tabs=xml
                var xmlDocument = new XmlDocument();
                DaoToastEvent daoToast=new DaoToastEvent();
                
                // lang=xml
                var toast = @"
                        <toast>
                            <visual>
                                <binding template='ToastText01'>
                                    <text id='1'>显示文本内容</text>
                                </binding>
                            </visual>
                        </toast>";
                var xml = daoToast.XmlData!=null ? daoToast.XmlData: toast ;
                var appName = DaoSysinfo.CurrentAppName;
                var name = daoToast.Name != null ? daoToast.Name : appName;
                xmlDocument.LoadXml(xml);

                var toastNotification = new ToastNotification(xmlDocument);
                var toastNotificationManagerForUser = ToastNotificationManager.GetDefault();
                var toastNotifier = toastNotificationManagerForUser.CreateToastNotifier(applicationId: name);
                toastNotifier.Show(toastNotification);
            }*/
        }
    }
}
