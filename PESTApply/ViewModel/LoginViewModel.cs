using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    public class LoginViewModel : ViewModelDefault
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LoginViewModel()
        {
            CaptchaBitmapImage = GetCaptcha();
        }

        #region 公共属性

        public Action<HttpClientResult, LoginResultModel> SuccessEvent { get; set; }

        /// <summary>
        /// 操作数据源
        /// </summary>
        public QuestSourceModel SourceModel
        {
            get { return sourceModel; }
            set { sourceModel = value; RaisePropertyChanged(() => SourceModel); }
        }
        private QuestSourceModel sourceModel;

        public string CaptchaCode
        {
            get { return captchaCode; }
            set { captchaCode = value; RaisePropertyChanged(() => CaptchaCode); }
        }
        private string captchaCode;

        public BitmapImage CaptchaBitmapImage
        {
            get { return captchaBitmapImage; }
            set { captchaBitmapImage = value; RaisePropertyChanged(() => CaptchaBitmapImage); }
        }
        private BitmapImage captchaBitmapImage;

        #endregion

        #region 私有属性

        /// <summary>
        /// 当前Cookies
        /// </summary>
        private CookieCollection CurrentCookies { get; set; }

        #endregion

        #region Event 验证码切换

        public RelayCommand OnCaptchaChange
        {
            get
            {
                if (onCaptchaChange == null)
                {
                    return new RelayCommand(() =>
                    {
                        CaptchaCode = "";
                        CaptchaBitmapImage = GetCaptcha();
                    });
                }
                return onCaptchaChange;
            }
            set { onCaptchaChange = value; }
        }
        private RelayCommand onCaptchaChange;

        #endregion

        #region Event 账号登录

        public RelayCommand OnLogin
        {
            get
            {
                if (onLogin == null)
                {
                    return new RelayCommand(() =>
                    {
                        var task = Task.Run(() =>
                        {
                            var result = AppHelper.GetLoginResult(SourceModel.Account, SourceModel.Password, CaptchaCode, CurrentCookies, (res, model) =>
                            {
                                SuccessEvent?.Invoke(res, model);
                                SendAction(MessengerConst.LoginWindowClose);
                            },
                            (res, model) =>
                            {
                                AppHelper.ShowMsg(model.msg, icon: System.Windows.MessageBoxImage.Error);
                            });
                        });
                    });
                }
                return onLogin;
            }
            set { onLogin = value; }
        }
        private RelayCommand onLogin;

        #endregion

        #region  辅助操作

        /// <summary>
        /// 获取验证码
        /// </summary>
        private BitmapImage GetCaptcha()
        {
            var result = AppHelper.GetCaptchResult();
            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                CurrentCookies = result.CookieCollection;
                return AppHelper.GetCaptchaBitmapImage(result);
            }
            return null;
        }

        #endregion
    }
}



