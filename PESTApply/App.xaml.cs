using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;
using GalaSoft.MvvmLight.Threading;

namespace PESTApply
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private IHosting appHosting { get; set; }

        public App()
        {
            DispatcherHelper.Initialize();
            appHosting = new AppHosting();
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppHelper.ProgramVerify();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //engine
            EngineHelper.Worker.Startup(appHosting);
            EngineHelper.Worker.OnStart();

            //init cache
            AppHelper.AppCacheInit();

            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("Error encountered! Please contact support." + Environment.NewLine + e.Exception.Message);
            //Shutdown(1);
            string errorMsg = e.Exception.ToString();
            if (e.Exception.InnerException != null)
            {
                errorMsg = e.Exception.InnerException.ToString();
            }
        }
    }
}
