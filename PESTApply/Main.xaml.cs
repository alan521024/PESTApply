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
using System.Windows.Shapes;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : DxuiWindow
    {
        public Main()
        {
            InitializeComponent();
            this.mainFrame.Content = new PESTApply.View.Home();
        }
    }
}
