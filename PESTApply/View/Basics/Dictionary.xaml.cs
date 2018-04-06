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

namespace PESTApply.View.Basics
{
    /// <summary>
    /// Dictionary.xaml 的交互逻辑
    /// </summary>
    public partial class Dictionary : DxuiWindow
    {
        public Dictionary()
        {
            InitializeComponent();
            this.DataContext = new DictionaryViewModel();

            InitUI();
            InitEven();
            InitData();
        }
        private void InitUI()
        {

        }

        private void InitEven()
        {
            //卸载当前(this)对象注册的所有MVVMLight消息
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void InitData()
        {

        }

    }
}
