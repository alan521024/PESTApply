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

namespace PESTApply.View
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : DxuiWindow
    {
        public Login(QuestSourceModel source, Action<HttpClientResult, LoginResultModel> successEvent)
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel() { SourceModel = source, SuccessEvent = successEvent };
            this.Title = string.Format("手动登录-{0}", source.Name);

            Messenger.Default.Register<TParam<string, dynamic>>(this, MessengerConst.OptionAction, m =>
            {
                switch (m.Param1)
                {
                    case MessengerConst.LoginWindowClose:
                        this.Close();
                        break;
                }
            });
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
