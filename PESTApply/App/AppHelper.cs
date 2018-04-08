using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Windows;
using System.Windows.Media.Imaging;
using NPOI.SS.UserModel;
using Newtonsoft.Json.Linq;
using GalaSoft.MvvmLight.Threading;
using DoubleX.Infrastructure.Utility;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 应用程序辅助类
    /// </summary>
    public static class AppHelper
    {
        #region 程序配置

        public static string UrlApplyPage = Properties.Settings.Default.ApplyPage;              //"http://order.etest.net.cn/select/index/12/0101";
        public static string UrlCaptcha = Properties.Settings.Default.Captcha;                  //"https://member.etest.net.cn/login/createCode";
        public static string UrlCaptchaValJuHe = Properties.Settings.Default.CaptchaValJuHe;    // "http://op.juhe.cn/vercode/index";
        public static string UrlLogin = Properties.Settings.Default.Login;                      // "https://member.etest.net.cn/login/loginSuccess";
        public static string UrlApply = Properties.Settings.Default.Apply;                      //"http://order.etest.net.cn/select/confirmation";
        public static string UrlAreas = Properties.Settings.Default.Areas;                      //"http://order.etest.net.cn/petSelect/getProvinces?cbd=knight&geoId=310000&proId=12";
        public static string UrlOrgAndSubject = Properties.Settings.Default.OrgAndSubject;      // "http://order.etest.net.cn/select/getAttr?cbd=knight";
        public static string UrlPackage = Properties.Settings.Default.Package;                  // "http://order.etest.net.cn/select/getList?cbd=knight&";

        public static string ValueAccount = Properties.Settings.Default.Account;                //"1161536884@qq.com";
        public static string ValuePassword = Properties.Settings.Default.Password;              //"123456fan";
        public static string ValueProId = Properties.Settings.Default.ProId;                    //"12";
        public static string ValueNodeCode = Properties.Settings.Default.NodeCode;              //"0101";
        public static string ValueStudentType = Properties.Settings.Default.StudentType;        // "08";
        public static string ValueMainArea = Properties.Settings.Default.MainArea;              //310000";
        public static string ValuePaymentType = Properties.Settings.Default.PaymentType;        //"1";
        public static string ValueJuHeKey1 = Properties.Settings.Default.JuHeKey1;              //"01bf2a9cd67d8dccd70cfd8a76aee77b";
        public static string ValueJuHeKey2 = Properties.Settings.Default.JuHeKey2;              //"1004";

        public static string XPathTimes = Properties.Settings.Default.TimesPath;               //"//dd[@id='time']";

        #endregion

        #region 程序校验

        public static void ProgramVerify()
        {
            Task.Run(() =>
            {
                try
                {
                    var options = new HttpClientOptions();
                    options.URL = "http://localhost:8101/api/app/verify?appCode=980001981"; //"http://verify.dev-tool.net/api/app/verify?appCode=980001981";
                    options.Method = "Get";
                    var result = new HttpWebClientUtility().Request(options);
                    if (result != null && !string.IsNullOrWhiteSpace(result.Content))
                    {
                        try
                        {
                            //{"code":0,"message":null,"obj":true}
                            var obj = JsonHelper.Deserialize<JObject>(result.Content);
                            if (obj != null && !BoolHelper.Get(obj.GetItem("obj")))
                            {
                                Environment.Exit(0);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            });
        }

        #endregion

        #region 应用缓存

        /// <summary>
        /// 应用缓存初始
        /// </summary>
        public static void AppCacheInit()
        {
            var quests = EngineHelper.Resolve<IQuestService>().GetAll();
            var flows = EngineHelper.Resolve<IQuestFlowService>().GetAll();

            quests.ForEach(item =>
            {
                item.Flows = new List<QuestFlowEntity>();
                var currents = flows.Where(x => x.QuestId == item.Id).ToList();
                item.Flows = currents?.ToList();
            });

            CachingHelper.Set<List<QuestEntity>>(AppConst.CacheQuestList, quests);
        }

        /// <summary>
        /// 获取所有任务配置
        /// </summary>
        public static List<QuestEntity> GetQuests()
        {
            return CachingHelper.Get<List<QuestEntity>>(AppConst.CacheQuestList);
        }

        /// <summary>
        /// 获取任务配置(ById)
        /// </summary>
        public static QuestEntity GetQuest(Guid id)
        {
            return GetQuests().FirstOrDefault(x => x.Id == id);
        }

        #endregion

        #region 控件操作

        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="msg"></param>
        public static MessageBoxResult ShowMsg(string msg, string title = "提示", MessageBoxButton btn = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            if (!string.IsNullOrWhiteSpace(msg) && !(msg.EndsWith("?") || msg.EndsWith("？")))
            {
                msg = msg.TrimEnd('!', '！', '.', '。', ',', '，') + "！";
            }
            return System.Windows.MessageBox.Show(msg, title, btn, icon);
        }

        #endregion

        #region 验证码操作

        /// <summary>
        /// 获取验证码请求结果
        /// </summary>
        /// <returns></returns>
        public static HttpClientResult GetCaptchResult()
        {
            var options = new HttpClientOptions();
            options.URL = AppHelper.UrlCaptcha;
            options.Method = "GET";
            options.ResultType = EnumHttpData.Base64;
            return new HttpWebClientUtility().Request(options);
        }

        /// <summary>
        /// 获取验证码位图
        /// </summary>
        /// <returns></returns>
        public static BitmapImage GetCaptchaBitmapImage(HttpClientResult result = null)
        {
            BitmapImage bmp = null;
            if (result == null)
            {
                result = GetCaptchResult();
            }
            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                try
                {
                    bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(result.Bytes);
                    bmp.EndInit();
                }
                catch
                {
                    bmp = null;
                }
            }
            return bmp;
        }

        #endregion

        #region 账号操作

        /// <summary>
        /// excel文件导入
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExcelSourceImport(string filePath, Func<int, int, int, IRow, IRow, bool> func)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            IRow head = null;
            FilesHelper.ImportExcel(filePath, (sheetIndex, rowIndex, total, row) =>
            {
                if (row == null || (row != null && row.Cells == null))
                {
                    return false;
                }
                if (sheetIndex != 0)
                {
                    return false;
                }

                if (rowIndex == 0)
                {
                    head = row;
                    return true;
                }
                return func(sheetIndex, rowIndex, total, head, row);
            });
        }

        /// <summary>
        /// 账号登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="captcha"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static HttpClientResult GetLoginResult(string account, string password, string captcha, CookieCollection cookies, Action<HttpClientResult, LoginResultModel> success = null, Action<HttpClientResult, LoginResultModel> error = null)
        {
            var options = new HttpClientOptions();
            options.URL = AppHelper.UrlLogin;
            options.Method = "POST";
            options.ContentType = "application/x-www-form-urlencoded";
            options.Accept = "*/*";
            if (!VerifyHelper.IsEmpty(cookies))
            {
                options.CookieCollection = cookies;
            }

            var loginParam = new List<KeyValueModel>();
            loginParam.Add(new KeyValueModel("threeView", "requirePrams,requirePrams"));
            loginParam.Add(new KeyValueModel("returnUrl", ""));
            loginParam.Add(new KeyValueModel("modelValue", ""));
            loginParam.Add(new KeyValueModel("id", ""));
            loginParam.Add(new KeyValueModel("loginName", account));
            loginParam.Add(new KeyValueModel("loginPwd", password));
            loginParam.Add(new KeyValueModel("account", account));
            loginParam.Add(new KeyValueModel("password", password));
            loginParam.Add(new KeyValueModel("verificationCode", captcha));
            options.PostData = StringHelper.GetFormStringByKeyValues(loginParam);

            var result = new HttpWebClientUtility().Request(options);
            if (result != null && !VerifyHelper.IsEmpty(result.Content))
            {
                var loginModel = JsonHelper.Deserialize<LoginResultModel>(result.Content);
                if (loginModel != null)
                {
                    if (loginModel.success)
                    {
                        success?.Invoke(result, loginModel);
                    }
                    else
                    {
                        error?.Invoke(result, loginModel);
                    }
                }
            }
            return result;
        }

        #endregion

    }
}
