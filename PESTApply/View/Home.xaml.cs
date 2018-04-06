using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;
using Basics = PESTApply.View.Basics;

namespace PESTApply.View
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            InitUI();
            InitEven();
            InitData();
        }

        private void InitUI()
        {

        }

        private void InitEven()
        {
            Messenger.Default.Register<TParam<string, EnumLoggerLev>>(this, MessengerConst.ConsoleWrite, m =>
            {
                ConsoleWrite(m.Param1, m.Param2);
            });
            Messenger.Default.Register<string>(this, MessengerConst.ConsoleClear, m =>
            {
                ConsoleClear();
            });
            Messenger.Default.Register<TParam<string, dynamic>>(this, MessengerConst.OptionAction, m =>
            {
                switch (m.Param1)
                {
                    case MessengerConst.ImportIng:

                        btnImportStart.Visibility = Visibility.Collapsed;
                        btnImportStop.Visibility = Visibility.Visible;

                        btnLoginStart.IsEnabled = false;
                        btnLoginStop.IsEnabled = false;

                        btnRefreshStart.IsEnabled = false;
                        btnRefreshStop.IsEnabled = false;

                        btnWorkStart.IsEnabled = false;
                        btnWorkStop.IsEnabled = false;

                        btnClear.IsEnabled = false;
                        btnSave.IsEnabled = false;

                        break;
                    case MessengerConst.ImportOver:

                        btnImportStart.Visibility = Visibility.Visible;
                        btnImportStop.Visibility = Visibility.Collapsed;

                        btnLoginStart.IsEnabled = true;
                        btnLoginStop.IsEnabled = true;

                        btnRefreshStart.IsEnabled = true;
                        btnRefreshStop.IsEnabled = true;

                        btnWorkStart.IsEnabled = true;
                        btnWorkStop.IsEnabled = true;

                        btnClear.IsEnabled = true;
                        btnSave.IsEnabled = true;

                        break;
                    case MessengerConst.LoginIng:

                        btnLoginStart.Visibility = Visibility.Collapsed;
                        btnLoginStop.Visibility = Visibility.Visible;

                        btnImportStart.IsEnabled = false;
                        btnImportStop.IsEnabled = false;

                        btnRefreshStart.IsEnabled = false;
                        btnRefreshStop.IsEnabled = false;

                        btnWorkStart.IsEnabled = false;
                        btnWorkStop.IsEnabled = false;

                        btnClear.IsEnabled = false;
                        btnSave.IsEnabled = false;

                        break;
                    case MessengerConst.LoginOver:

                        btnLoginStart.Visibility = Visibility.Visible;
                        btnLoginStop.Visibility = Visibility.Collapsed;

                        btnImportStart.IsEnabled = true;
                        btnImportStop.IsEnabled = true;

                        btnRefreshStart.IsEnabled = true;
                        btnRefreshStop.IsEnabled = true;

                        btnWorkStart.IsEnabled = true;
                        btnWorkStop.IsEnabled = true;

                        btnClear.IsEnabled = true;
                        btnSave.IsEnabled = true;

                        break;
                    case MessengerConst.LoginManual:
                        Login login = new Login(m.Param2.Source, m.Param2.SuccessEvent);
                        login.Owner = DesktopUIHelper.GetAncestor<Main>(this);
                        login.WindowStartupLocation = WindowStartupLocation.CenterOwner; // FormStartPosition.CenterParent;
                        login.ShowDialog();
                        break;
                    case MessengerConst.WorkIng:

                        btnWorkStart.Visibility = Visibility.Collapsed;
                        btnWorkStop.Visibility = Visibility.Visible;

                        btnImportStart.IsEnabled = false;
                        btnImportStop.IsEnabled = false;

                        btnLoginStart.IsEnabled = false;
                        btnLoginStop.IsEnabled = false;

                        btnRefreshStart.IsEnabled = false;
                        btnRefreshStop.IsEnabled = false;

                        btnClear.IsEnabled = false;
                        btnSave.IsEnabled = false;

                        break;
                    case MessengerConst.WorkOver:

                        btnWorkStart.Visibility = Visibility.Visible;
                        btnWorkStop.Visibility = Visibility.Collapsed;

                        btnImportStart.IsEnabled = true;
                        btnImportStop.IsEnabled = true;

                        btnLoginStart.IsEnabled = true;
                        btnLoginStop.IsEnabled = true;

                        btnRefreshStart.IsEnabled = true;
                        btnRefreshStop.IsEnabled = true;

                        btnClear.IsEnabled = true;
                        btnSave.IsEnabled = true;

                        break;
                    case MessengerConst.RefreshIng:

                        btnRefreshStart.Visibility = Visibility.Collapsed;
                        btnRefreshStop.Visibility = Visibility.Visible;

                        break;
                    case MessengerConst.RefreshOver:

                        btnRefreshStart.Visibility = Visibility.Visible;
                        btnRefreshStop.Visibility = Visibility.Collapsed;

                        break;
                    case MessengerConst.OpenDictionary:

                        Basics.Dictionary dictionary = new Basics.Dictionary();
                        dictionary.Owner = DesktopUIHelper.GetAncestor<Main>(this);
                        dictionary.WindowStartupLocation = WindowStartupLocation.CenterOwner; // FormStartPosition.CenterParent;
                        dictionary.ShowDialog();

                        break;
                }
            });

            //卸载当前(this)对象注册的所有MVVMLight消息
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void InitData()
        {

        }

        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="msg"></param>
        private void ConsoleWrite(string msg, EnumLoggerLev loggerLev = EnumLoggerLev.All)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TextBlock msgBlock = new TextBlock();
                msgBlock.Text = string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:sss"), msg);
                switch (loggerLev)
                {
                    case EnumLoggerLev.Info:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Green"));
                        break;
                    case EnumLoggerLev.Error:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        break;
                    case EnumLoggerLev.Warn:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFAE00"));
                        break;
                    default:
                        msgBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34495E"));
                        break;
                }
                spLoggin.Children.Add(msgBlock);
            });

            #region 操作日志，滚动至底部

            //ThreadPool.QueueUserWorkItem(sender =>
            //{
            //    while (true)
            //    {
            //        this.txtLog.Dispatcher.BeginInvoke((Action)delegate
            //        {
            //            this.txtLog.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff\r\n"));
            //            if (IsVerticalScrollBarAtBottom)
            //            {
            //                this.txtLog.ScrollToEnd();
            //            }
            //        });
            //        Thread.Sleep(600);
            //    }
            //});

            #endregion
        }

        /// <summary>
        /// 控制台清空
        /// </summary>
        private void ConsoleClear()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                spLoggin.Children.Clear();
            });
        }

    }
}
