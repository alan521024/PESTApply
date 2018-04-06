using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using NPOI.SS.UserModel;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// ViewModel 默认操作
    /// </summary>
    public class ViewModelDefault : ViewModelBase
    {
        /// <summary>
        /// 应用程序缓存
        /// </summary>
        public AppCache AppCache
        {
            get { return appCache; }
            set { appCache = value; RaisePropertyChanged(() => AppCache); }
        }
        private AppCache appCache = new AppCache();

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusText
        {
            get { return statusText; }
            set { statusText = value; RaisePropertyChanged(() => StatusText); }
        }
        private string statusText = "没有任务运行.....";

        /// <summary>
        /// 显示状态描述
        /// </summary>
        protected void ShowStatusText(string text, Action action = null, bool isConsoleWrite = true, EnumLoggerLev loggerLev = EnumLoggerLev.All)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                StatusText = text;
                if (isConsoleWrite)
                {
                    SendConsoleWrite(text, loggerLev: loggerLev);
                }
                action?.Invoke();
            });
        }

        /// <summary>
        /// 发送输出控制台
        /// </summary>
        /// <param name="text"></param>
        /// <param name="loggerLev"></param>
        protected void SendConsoleWrite(string text, EnumLoggerLev loggerLev = EnumLoggerLev.All)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send<TParam<string, EnumLoggerLev>>(new TParam<string, EnumLoggerLev>() { Param1 = text, Param2 = loggerLev }, MessengerConst.ConsoleWrite);
            });
        }

        /// <summary>
        /// 发送清空控制台
        /// </summary>
        /// <param name="text"></param>
        /// <param name="loggerLev"></param>
        protected void SendConsoleClear()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send<String>(null, MessengerConst.ConsoleClear);
            });
        }

        /// <summary>
        /// 发送首页操作
        /// </summary>
        /// <param name="actionName"></param>
        protected void SendAction(string actionName, dynamic param = null)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send<TParam<string, dynamic>>(new TParam<string, dynamic>() { Param1 = actionName, Param2 = param }, MessengerConst.OptionAction);
            });
        }

    }
}
