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
    public class HomeViewModel : ViewModelDefault
    {
        protected IDictionaryService dictionaryService;
        protected IPETSPackageService petsPackageService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HomeViewModel()
        {
            dictionaryService = EngineHelper.Resolve<IDictionaryService>();
            petsPackageService = EngineHelper.Resolve<IPETSPackageService>();
        }

        #region 公共属性

        /// <summary>
        /// 操作数据源
        /// </summary>
        public ObservableCollection<QuestSourceModel> SourceList
        {
            get { return sourceList; }
            set { sourceList = value; RaisePropertyChanged(() => SourceList); }
        }
        private ObservableCollection<QuestSourceModel> sourceList = new ObservableCollection<QuestSourceModel>();

        /// <summary>
        /// 并行任务数
        /// </summary>
        public int TaskCount
        {
            get { return taskCount; }
            set { taskCount = value; RaisePropertyChanged(() => TaskCount); }
        }
        private int taskCount = 5;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; RaisePropertyChanged(() => RetryCount); }
        }
        private int retryCount = 5;

        /// <summary>
        /// 重试次数
        /// </summary>
        public bool IsErrorGoOn
        {
            get { return isErrorGoOn; }
            set { isErrorGoOn = value; RaisePropertyChanged(() => IsErrorGoOn); }
        }
        private bool isErrorGoOn = true;

        #endregion

        #region 私有属性

        /// <summary>
        /// 是否停止导入
        /// </summary>
        private bool IsImportStop { get; set; } = false;

        /// <summary>
        /// 是否批量登录
        /// </summary>
        private bool IsLogining { get; set; } = false;

        /// <summary>
        /// 是否正在任务
        /// </summary>
        private bool IsWorking { get; set; } = false;

        #endregion

        #region 导入操作

        public RelayCommand OnImportStart
        {
            get
            {
                if (onImportStart == null)
                {
                    return new RelayCommand(() =>
                    {
                        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                        dlg.Multiselect = false;
                        dlg.DefaultExt = ".xlsx";
                        dlg.Filter = " Excel文件(.txt)|*.xlsx";
                        Nullable<bool> result = dlg.ShowDialog();
                        if (result.Value)
                        {
                            var task = Task.Factory.StartNew(() =>
                            {
                                ShowStatusText(string.Format("准备导入文件{0}", dlg.FileName), () => { SendAction(MessengerConst.ImportIng); });

                                var imports = new List<QuestSourceModel>();

                                //导入
                                AppHelper.ExcelSourceImport(dlg.FileName, (sheet, index, total, head, row) =>
                                {
                                    //中断操作
                                    if (IsImportStop)
                                    {
                                        return false;
                                    }

                                    var item = ExcelRowToSource(head, row);
                                    if (item != null)
                                    {
                                        item.No = imports.Count() + 1;
                                        imports.Add(item);
                                        ShowStatusText(string.Format("正在导入 {0} / {1} ", index + 1, total), isConsoleWrite: false);
                                    }
                                    else
                                    {
                                        ShowStatusText(string.Format("第 {0} 行 导入失败", index + 1), null, true, EnumLoggerLev.Error);
                                    }

                                    return true;
                                });

                                //匹配
                                ExcelMatchDictionary(imports);

                                ShowStatusText(IsImportStop ? "停止导入" : "导入结束", () =>
                                {
                                    SendAction(MessengerConst.ImportOver, IsImportStop);
                                    SourceList = IsImportStop ? new ObservableCollection<QuestSourceModel>() : new ObservableCollection<QuestSourceModel>(imports);
                                    IsImportStop = false;
                                });
                            });
                        }
                    });
                }
                return onImportStart;
            }
            set { onImportStart = value; }
        }
        private RelayCommand onImportStart;

        public RelayCommand OnImportStop
        {
            get
            {
                if (onImportStop == null)
                {
                    return new RelayCommand(() =>
                    {
                        IsImportStop = true;
                    });
                }
                return onImportStop;
            }
            set { onImportStop = value; }
        }
        private RelayCommand onImportStop;

        #endregion

        #region 清空操作

        public RelayCommand OnClear
        {
            get
            {
                if (onClear == null)
                {
                    return new RelayCommand(() =>
                    {
                        ShowStatusText("清空任务数据", () =>
                        {
                            SourceList = new ObservableCollection<QuestSourceModel>();
                            SendConsoleClear();
                            AppHelper.ShowMsg("任务数据及控制台输入已清空");
                        });
                    });
                }
                return onClear;
            }
            set { onClear = value; }
        }
        private RelayCommand onClear;


        public RelayCommand OnConsoleClear
        {
            get
            {
                if (onConsoleClear == null)
                {
                    return new RelayCommand(() =>
                    {
                        SendConsoleClear();
                    });
                }
                return onConsoleClear;
            }
            set { onConsoleClear = value; }
        }
        private RelayCommand onConsoleClear;


        #endregion

        #region 保存操作

        public RelayCommand OnSave
        {
            get
            {
                if (onSave == null)
                {
                    return new RelayCommand(() =>
                    {
                        if (CheckSourceIsEmpty())
                        {
                            return;
                        }

                        AppHelper.ShowMsg("暂不实现");
                    });
                }
                return onSave;
            }
            set { onSave = value; }
        }
        private RelayCommand onSave;

        #endregion

        #region 登录操作

        public RelayCommand OnLoginStart
        {
            get
            {
                if (onLoginStart == null)
                {
                    return new RelayCommand(() =>
                    {
                        if (CheckSourceIsEmpty())
                        {
                            return;
                        }

                        var task = Task.Run(() =>
                        {
                            ShowStatusText("开始批量登录", () =>
                            {
                                SendAction(MessengerConst.LoginIng);
                            });

                            foreach (var item in sourceList)
                            {
                                if (!item.IsLogin)
                                {
                                    item.RetryTotal = 0; //重置重试次数
                                }
                            }

                            IsLogining = true;

                            while (IsLogining)
                            {
                                var works = SourceList.Where(x => !x.IsLogin && x.RetryTotal < RetryCount).ToList();
                                if (!IsLogining || VerifyHelper.IsEmpty(works))
                                {
                                    IsLogining = false;
                                    break;
                                }

                                foreach (var item in works)
                                {
                                    item.RetryTotal++;
                                    try
                                    {
                                        WorkLogin(item);
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowStatusText(string.Format("异常：{0}", ExceptionHelper.GetMessage(ex)));
                                        if (!isErrorGoOn)
                                        {
                                            IsLogining = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!IsLogining)
                            {
                                ShowStatusText("结束批量登录", () => { SendAction(MessengerConst.LoginOver); });
                            }
                        });

                    });
                }
                return onLoginStart;
            }
            set { onLoginStart = value; }
        }
        private RelayCommand onLoginStart;

        public RelayCommand OnLoginStop
        {
            get
            {
                if (onLoginStop == null)
                {
                    return new RelayCommand(() =>
                    {
                        ShowStatusText("停止批量登录", () =>
                        {
                            IsLogining = false;
                            SendAction(MessengerConst.LoginOver);
                        }, true);
                    });
                }
                return onLoginStop;
            }
            set { onLoginStop = value; }
        }
        private RelayCommand onLoginStop;

        #endregion

        #region 执行操作

        public RelayCommand OnWorkStart
        {
            get
            {
                if (onWorkStart == null)
                {

                    return new RelayCommand(() =>
                    {
                        if (CheckSourceIsEmpty())
                        {
                            return;
                        }

                        var task = Task.Run(() =>
                        {
                            ShowStatusText("开始批量处理", () =>
                            {
                                SendAction(MessengerConst.WorkIng);
                            });

                            foreach (var item in sourceList)
                            {
                                if (!item.IsWork)
                                {
                                    item.RetryTotal = 0; //重置重试次数
                                }
                            }

                            IsWorking = true;

                            while (IsWorking)
                            {
                                var works = SourceList.Where(x => !x.IsWork && x.RetryTotal < RetryCount).ToList();
                                if (!IsWorking || VerifyHelper.IsEmpty(works))
                                {
                                    IsWorking = false;
                                    break;
                                }

                                if (TaskCount == 0)
                                {
                                    //保证至少一次任务
                                    TaskCount = 1;
                                }

                                #region 多线程操作

                                var batchCount = works.Count / TaskCount;
                                if (works.Count % TaskCount > 0)
                                {
                                    batchCount++;
                                }
                                Parallel.For(0, batchCount, i =>
                                {
                                    var currentStart = i * TaskCount;
                                    var currentEnd = currentStart + TaskCount;
                                    if (currentEnd > works.Count)
                                    {
                                        currentEnd = works.Count;
                                    }

                                    Parallel.For(currentStart, currentEnd, j =>
                                    {
                                        var item = works[j];
                                        item.RetryTotal++;
                                        try
                                        {
                                            WorkApply(item);
                                        }
                                        catch (Exception ex)
                                        {
                                            ShowStatusText(string.Format("异常：{0}", ExceptionHelper.GetMessage(ex)));
                                            if (!isErrorGoOn)
                                            {
                                                IsWorking = false;
                                            }
                                        }
                                    });
                                });

                                #endregion
                            }

                            if (!IsWorking)
                            {
                                ShowStatusText("结束批量处理", () => { SendAction(MessengerConst.WorkOver); });
                            }
                        });

                    });
                }
                return onWorkStart;
            }
            set { onWorkStart = value; }
        }
        private RelayCommand onWorkStart;

        public RelayCommand OnWorkStop
        {
            get
            {
                if (onWorkStop == null)
                {
                    return new RelayCommand(() =>
                    {
                        ShowStatusText("停止批量申请", () =>
                        {
                            IsWorking = false;
                            SendAction(MessengerConst.WorkOver);
                        }, true);
                    });
                }
                return onWorkStop;
            }
            set { onWorkStop = value; }
        }
        private RelayCommand onWorkStop;

        #endregion

        #region 刷新操作

        public RelayCommand OnRefreshStart
        {
            get
            {
                if (onRefreshStart == null)
                {
                    return new RelayCommand(() =>
                    {
                        if (CheckSourceIsEmpty())
                        {
                            return;
                        }

                        ShowStatusText("开始状态刷新", () =>
                        {
                            SendAction(MessengerConst.RefreshIng);
                        }, true);
                    });
                }
                return onRefreshStart;
            }
            set { onRefreshStart = value; }
        }
        private RelayCommand onRefreshStart;

        public RelayCommand OnRefreshStop
        {
            get
            {
                if (onRefreshStop == null)
                {
                    return new RelayCommand(() =>
                    {
                        ShowStatusText("停止状态刷新", () =>
                        {
                            SendAction(MessengerConst.RefreshOver);
                        }, true);
                    });
                }
                return onRefreshStop;
            }
            set { onRefreshStop = value; }
        }
        private RelayCommand onRefreshStop;

        #endregion

        #region 数据维护

        public RelayCommand OnDictionary
        {
            get
            {
                if (onDictionary == null)
                {
                    return new RelayCommand(() =>
                    {
                        //if (CheckSourceIsEmpty())
                        //{
                        //    return;
                        //}
                        SendAction(MessengerConst.OpenDictionary);
                    });
                }
                return onDictionary;
            }
            set { onDictionary = value; }
        }
        private RelayCommand onDictionary;

        #endregion

        #region 表格双击

        public RelayCommand<QuestSourceModel> OnGridDoubleClick
        {
            get
            {
                if (onGridDoubleClick == null)
                {
                    return new RelayCommand<QuestSourceModel>((item) =>
                    {
                        if (item != null)
                        {
                            Action<HttpClientResult, LoginResultModel> successEvent = (res, model) =>
                            {
                                SetSourceLoginStatus(item, res, model);
                            };
                            SendAction(MessengerConst.LoginManual, new { Source = item, SuccessEvent = successEvent });
                        }
                    });
                }
                return onGridDoubleClick;
            }
            set { onGridDoubleClick = value; }
        }
        private RelayCommand<QuestSourceModel> onGridDoubleClick;

        #endregion

        #region  辅助操作

        /// <summary>
        /// 将Excel 数据源 Row 转为  QuestSourceModel
        /// </summary>
        private QuestSourceModel ExcelRowToSource(IRow head, IRow row)
        {
            QuestSourceModel model = null;
            if (row != null)
            {
                model = new QuestSourceModel();
                model.Id = Guid.NewGuid();
                model.Name = StringHelper.Get(row.Cells[1]);
                model.Account = StringHelper.Get(row.Cells[2]);
                model.Password = StringHelper.Get(row.Cells[3]);

                model.Cookies = new CookieCollection();
                model.Message = "";
                model.Examination = "";
                model.UserId = "";
                model.Examination = "";
                model.RelationshipValue = "";
                model.AreaValue = "";
                model.SubjectValue = "";
                model.ExamValue = "";
                model.PackageValue = "";

                model.ExamText = StringHelper.Get(row.Cells[6]);
                model.AreaText = StringHelper.Get(row.Cells[7]);
                model.OrgText = StringHelper.Get(row.Cells[8]);
                model.CategoryText = StringHelper.Get(row.Cells[9]);

                model.RetryTotal = 0;
                model.IsLogin = false;
                model.IsWork = false;
            }
            return model;
        }

        /// <summary>
        ///  将Excel 数据源 匹配数据字典及考试信息
        /// </summary>
        private void ExcelMatchDictionary(List<QuestSourceModel> sources)
        {
            if (VerifyHelper.IsEmpty(sources))
                return;

            var dictionarys = dictionaryService.GetAll();
            var packages = petsPackageService.GetAll();

            var examTimes = dictionarys.Where(x => x.Genre == AppConst.DictionaryTimes).ToList();
            var areas = dictionarys.Where(x => x.Genre == AppConst.DictionaryArea).ToList();
            var orgs = dictionarys.Where(x => x.Genre == AppConst.DictionaryOrganiz).ToList();
            var categorys = dictionarys.Where(x => x.Genre == AppConst.DictionaryCategory).ToList();
            var subjects = dictionarys.Where(x => x.Genre == AppConst.DictionarySubject).ToList();

            foreach (var item in sources)
            {
                ShowStatusText(string.Format("{0}[匹配],正在匹配考试信息", item.Name));

                var examQuery = examTimes.Where(x => x.Name == item.ExamText).ToList();
                var areaQuery = areas.Where(x => x.Name == item.AreaText).ToList();
                var orgQuery = orgs.Where(x => x.Name == item.OrgText).ToList();
                var categoryQuery = categorys.Where(x => x.Name == item.CategoryText).ToList();

                #region 数据校验1

                if (examQuery.Count() != 1)
                {
                    item.Examination = string.Format("error: exam({0})", examQuery.Count());
                    continue;
                }
                if (areaQuery.Count() != 1)
                {
                    item.Examination = string.Format("error: area({0})", areaQuery.Count());
                    continue;
                }
                if (orgQuery.Count() != 1)
                {
                    item.Examination = string.Format("error: org({0})", orgQuery.Count());
                    continue;
                }
                if (categoryQuery.Count() != 1)
                {
                    item.Examination = string.Format("error: category({0})", categoryQuery.Count());
                    continue;
                }

                #endregion

                var packageQuery = packages.Where(x => x.ExamId == examQuery.FirstOrDefault().Value &&
                x.OrgGeo == areaQuery.FirstOrDefault().Value &&
                x.RelationshipId == orgQuery.FirstOrDefault().Value &&
                x.CategoryId == categoryQuery.FirstOrDefault().Value);

                ///单科考(笔 / 口)  或  多科考(笔 + 口)
                if (packageQuery.Count() > 2 || packageQuery.Count() < 1)
                {
                    item.Examination = string.Format("error: package({0})", packageQuery.Count());
                    continue;
                }

                string joinStr = packageQuery.Count() == 2 ? "|" : "";

                item.ExamValue = string.Join(joinStr, packageQuery.Select(x => x.ExamId));
                item.ExamNameValue = string.Join(joinStr, packageQuery.Select(x => x.ExamDate));
                item.AreaValue = string.Join(joinStr, packageQuery.Select(x => x.OrgGeo));
                item.RelationshipValue = string.Join(joinStr, packageQuery.Select(x => x.RelationshipId));
                item.CategoryValue = string.Join(joinStr, packageQuery.Select(x => x.CategoryId));
                item.SubjectValue = string.Join(joinStr, packageQuery.Select(x => x.SubjectId));
                item.PackageValue = string.Join(joinStr, packageQuery.Select(x => x.PackageId));
                item.PackageIdValue = string.Join(joinStr, packageQuery.Select(x => x.Id.ToString()));

                var packageModel = packageQuery.FirstOrDefault();
                item.Examination = string.Format("{0}/{1}/{2}/{3}", packageModel.FullName, packageModel.OrgName,
                    string.Join(joinStr, packageQuery.Select(x => x.SubjectName)),
                    string.Join(joinStr, packageQuery.Select(x => x.ExamDate)));
            }
        }


        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="model"></param>
        private void WorkLogin(QuestSourceModel model)
        {
            if (model == null)
                return;

            if (model.IsLogin)
                return;

            string captchaBase64 = string.Empty;
            string captchaCode = string.Empty;

            //(0)获取验证码，及第一次请求时Cookies
            captchaBase64 = StepByOpenAndGetCaptchaBase64(model);

            //(1)验证码识别
            if (VerifyHelper.IsEmpty(captchaBase64))
            {
                model.Message = "获取验证码失败";
            }
            else
            {
                captchaCode = StepByGetCaptchaCodeByJUHE(model, captchaBase64);
            }

            //(2)登录操作
            if (VerifyHelper.IsEmpty(captchaCode))
            {
                model.Message = "验证码识别失败";
            }
            else
            {
                StepByLogin(model, captchaCode);
            }

            ShowStatusText(string.Format("{0}[登录]（{1}）", model.Name, model.IsLogin ? "成功" : "失败"),
                loggerLev: model.IsLogin ? EnumLoggerLev.Info : EnumLoggerLev.Error);
        }

        /// <summary>
        /// 申请执行
        /// </summary>
        /// <param name="model"></param>
        private void WorkApply(QuestSourceModel model)
        {
            if (model == null)
                return;

            if (model.IsWork)
                return;

            //登录处理
            if (!model.IsLogin)
            {
                WorkLogin(model);
            }

            //登陆失败
            if (!model.IsLogin)
            {
                return;
            }

            //申请处理
            StepByApply(model);
        }


        /// <summary>
        /// 操作步聚：访问站点，获取Cookies及验证码Base64
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string StepByOpenAndGetCaptchaBase64(QuestSourceModel model)
        {
            ShowStatusText(string.Format("{0}[登录],正在读取验证码", model.Name));
            string codeBase64 = string.Empty;
            var result = AppHelper.GetCaptchResult();
            if (!VerifyHelper.IsEmpty(result.Content))
            {
                model.Cookies = result.CookieCollection;
                codeBase64 = result.Content;
            }
            ShowStatusText(string.Format("{0}[登录],验证码读取 {1}", model.Name, !VerifyHelper.IsEmpty(codeBase64) ? "成功" : "失败"));
            return codeBase64;
        }

        /// <summary>
        /// 操作步聚：获取验证码内容(从聚合API接口)
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private string StepByGetCaptchaCodeByJUHE(QuestSourceModel model, string base64)
        {
            ShowStatusText(string.Format("{0}[登录],自动识别验证码", model.Name));

            string captchaCodeValue = string.Empty;

            if (!VerifyHelper.IsEmpty(base64))
            {
                var options = new HttpClientOptions();
                var client = new HttpWebClientUtility();
                options.URL = AppHelper.UrlCaptchaValJuHe;
                options.Method = "POST";
                options.ContentType = "application/x-www-form-urlencoded";
                options.PostData = string.Format("key={0}&codeType={1}&base64Str={2}", AppHelper.ValueJuHeKey1, AppHelper.ValueJuHeKey2, CodingHelper.UrlEncoding(base64));
                var result = client.Request(options);
                if (!VerifyHelper.IsEmpty(result.Content))
                {
                    var obj = JsonHelper.Deserialize<JObject>(result.Content);
                    if (obj != null && StringHelper.Get(obj["error_code"]) == "0")
                    {
                        captchaCodeValue = StringHelper.Get(obj["result"]);
                    }
                }
            }

            ShowStatusText(string.Format("{0}[登录],验证码识别 {1}", model.Name, !VerifyHelper.IsEmpty(captchaCodeValue) ? "成功" : "失败"));

            return captchaCodeValue;
        }

        /// <summary>
        /// 操作步聚：使用账号密码验证码登录系统
        /// </summary>
        /// <param name="model"></param>
        /// <param name="captchaValue"></param>
        private void StepByLogin(QuestSourceModel model, string captchaValue)
        {
            ShowStatusText(string.Format("{0}[登录],正在登录系统", model.Name));

            bool isSuccess = false;

            if (!VerifyHelper.IsEmpty(captchaValue))
            {
                var result = AppHelper.GetLoginResult(model.Account, model.Password, captchaValue, model.Cookies, (res, loginModel) =>
                {
                    SetSourceLoginStatus(model, res, loginModel);

                }, (res, loginModel) =>
                {
                    model.Message = "登录失败";
                    model.IsLogin = false;
                });
            }

            ShowStatusText(string.Format("{0}[登录], {1}", model.Name, isSuccess ? "成功" : "失败"));

            //{
            // "success" : true,
            //  "msg" : "登录成功",
            //  "obj" : {
            //    "returnUrl" : "?UUID=94b3bf6d002f4fe586e122ce93c0299d&name=%E5%B0%B9%E4%BC%8A%E5%A8%9C&mail=1161536884%40qq.com",
            //    "akashaUrl" : "http://order.etest.net.cn/order/mySignUp",
            //    "formData" : null,
            //    "userName" : "尹伊娜"
            //  },
            //  "token" : ""
            // }
        }

        /// <summary>
        /// 操作步聚：报名申请
        /// </summary>
        /// <param name="model"></param>
        private void StepByApply(QuestSourceModel model)
        {
            ShowStatusText(string.Format("{0}[申请],正在申请", model.Name));

            var options = new HttpClientOptions();
            var client = new HttpWebClientUtility();
            options.URL = AppHelper.UrlApply;
            options.Method = "POST";
            options.ContentType = "application/x-www-form-urlencoded";
            options.CookieCollection = model.Cookies;

            List<KeyValueModel> datas = new List<KeyValueModel>();

            var examValueArr = model.ExamValue.Split('|');
            var examNameArr = model.ExamNameValue.Split('|');
            var areaValueArr = model.AreaValue.Split('|');
            var relationshipValueArr = model.RelationshipValue.Split('|');
            var categoryValueArr = model.CategoryValue.Split('|');
            var subjectValueArr = model.SubjectValue.Split('|');
            var packageValueArr = model.PackageValue.Split('|');

            datas.Add(new KeyValueModel("scoreId", ""));
            datas.Add(new KeyValueModel("uId", model.UserId));
            datas.Add(new KeyValueModel("proId", AppHelper.ValueProId));
            datas.Add(new KeyValueModel("nodeCode", AppHelper.ValueNodeCode));
            datas.Add(new KeyValueModel("studentType", AppHelper.ValueStudentType));
            datas.Add(new KeyValueModel("paymentType", AppHelper.ValuePaymentType));

            datas.Add(new KeyValueModel("examId", examValueArr[0]));
            datas.Add(new KeyValueModel("orgGeo", areaValueArr[0]));
            datas.Add(new KeyValueModel("relationshipId", relationshipValueArr[0]));
            datas.Add(new KeyValueModel("packageId", string.Format("{0}#", categoryValueArr[0])));


            for (var i = 0; i < model.PackageIdValue.Split('|').Length; i++)
            {
                datas.Add(new KeyValueModel("regList[0].examId", examValueArr[i]));
                datas.Add(new KeyValueModel("regList[0].examDate", examNameArr[i]));
                datas.Add(new KeyValueModel("regList[0].orgGeo", areaValueArr[i]));
                datas.Add(new KeyValueModel("regList[0].relationshipId", relationshipValueArr[i]));
                datas.Add(new KeyValueModel("regList[0].categoryId", categoryValueArr[i]));
                datas.Add(new KeyValueModel("regList[0].subjectId", subjectValueArr[i]));
                datas.Add(new KeyValueModel("regList[0].packageId", packageValueArr[i]));
            }
            options.PostData = StringHelper.GetFormStringByKeyValues(datas);

            var result = client.Request(options);
            if (!VerifyHelper.IsEmpty(result.Content))
            {
                var resModel = JsonHelper.Deserialize<ApplyResultModel>(result.Content);
                if (resModel != null && resModel.success)
                {
                    model.Message = "申请成功";
                    model.RetryTotal = 0;
                    model.IsWork = true;
                }
                else
                {
                    model.Message = resModel.msg;
                }
            }

            ShowStatusText(string.Format("{0}[申请]（{1}）", model.Name, model.IsWork ? "成功" : "失败"),
                loggerLev: model.IsWork ? EnumLoggerLev.Info : EnumLoggerLev.Error);
        }


        /// <summary>
        /// 检查数据源是否为空
        /// </summary>
        /// <returns></returns>
        private bool CheckSourceIsEmpty()
        {
            if (VerifyHelper.IsEmpty(SourceList))
            {
                AppHelper.ShowMsg("请先导入数据源");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置登录成功后的登录状态
        /// </summary>
        public void SetSourceLoginStatus(QuestSourceModel source, HttpClientResult result, LoginResultModel loginModel)
        {
            if (!VerifyHelper.IsEmpty(loginModel) && !VerifyHelper.IsEmpty(loginModel.obj) && loginModel.success)
            {
                var resultObj = loginModel.obj as JObject;
                var paramList = StringHelper.GetToListKeyValue(resultObj.GetItem("returnUrl").Replace("?", ""), listSplit: '&');
                if (!VerifyHelper.IsEmpty(paramList) && paramList.Where(x => x.Key.ToLower() == "uuid").Count() > 0)
                {
                    source.Message = "";
                    source.RetryTotal = 0;
                    source.Cookies = result.CookieCollection;
                    source.IsLogin = true;
                    source.UserId = paramList.Where(x => x.Key.ToLower() == "uuid").FirstOrDefault().Value;
                }
            }
        }

        #endregion

        #region 申请资料1

        ///****笔试+口试******/
        //<tr>
        //    <input type =\"hidden\" name=\"regList[0].categoryId\" value=\"9c98e0ce6416473596c3f8df265d681c\">
        //    <input type =\"hidden\" name=\"regList[0].packageId\" value=\"12A272\">
        //    <input type =\"hidden\" name=\"regList[0].orgGeo\" value=\"110000110108\">
        //    <input type =\"hidden\" name=\"regList[0].relationshipId\" value=\"12000100090006\">
        //    <input type =\"hidden\" name=\"regList[0].examId\" value=\"201812001\">
        //    <input type =\"hidden\" name=\"regList[0].examDate\" value=\"2018.03.31 上午\">
        //    <input type =\"hidden\" name=\"regList[0].subjectId\" value=\"120902\">

        //    <input type =\"hidden\" name=\"regList[0].categoryId\" value=\"9c98e0ce6416473596c3f8df265d681c\">
        //    <input type =\"hidden\" name=\"regList[0].packageId\" value=\"12A272\">
        //    <input type =\"hidden\" name=\"regList[0].orgGeo\" value=\"110000110108\">
        //    <input type =\"hidden\" name=\"regList[0].relationshipId\" value=\"12000100090006\">
        //    <input type =\"hidden\" name=\"regList[0].examId\" value=\"201812001\">
        //    <input type =\"hidden\" name=\"regList[0].examDate\" value=\"2018.03.31 下午 ~ 2018.04.01 下午\">
        //    <input type =\"hidden\" name=\"regList[0].subjectId\" value=\"120901\">

        //    <input type =\"hidden\" name=\"scoreId\" value=\"\">
        //    <input type =\"hidden\" name=\"uId\" value=\"94b3bf6d002f4fe586e122ce93c0299d\">
        //    <input type =\"hidden\" name=\"proId\" value=\"12\">
        //    <input type =\"hidden\" name=\"nodeCode\" value=\"0101\">
        //    <input type =\"hidden\" name=\"studentType\" value=\"08\">
        //    <input type =\"hidden\" name=\"examId\" value=\"201812001\">
        //    <input type =\"hidden\" name=\"orgGeo\" value=\"110000110108\">
        //    <input type =\"hidden\" name=\"paymentType\" value=\"1\">
        //    <input type =\"hidden\" name=\"packageId\" value=\"9c98e0ce6416473596c3f8df265d681c#\">
        //    <input type =\"hidden\" name=\"relationshipId\" value=\"12000100090006\">
        //    </tr>


        ////表单JSON

        //http://order.etest.net.cn/select/confirmation
        //examId	201812001
        //nodeCode	0101
        //orgGeo	110000110108
        //packageId	9c98e0ce6416473596c3f8df265d681c#
        //paymentType	1
        //proId	12
        //regList[0].categoryId	{…}
        //0	9c98e0ce6416473596c3f8df265d681c
        //1	9c98e0ce6416473596c3f8df265d681c
        //regList[0].examDate	{…}
        //0	2018.03.31+上午
        //1	2018.03.31+下午+~+2018.04.01+下午
        //regList[0].examId	{…}
        //0	201812001
        //1	201812001
        //regList[0].orgGeo	{…}
        //0	110000110108
        //1	110000110108
        //regList[0].packageId	{…}
        //0	12A272
        //1	12A272
        //regList[0].relationshipId	{…}
        //0	12000100090006
        //1	12000100090006
        //regList[0].subjectId	{…}
        //0	120902
        //1	120901
        //relationshipId	12000100090006
        //scoreId
        //studentType 08
        //uId	94b3bf6d002f4fe586e122ce93c0299d



        #endregion

        #region 申请资料2

        //<tr>
        //<input type =\"hidden\" name=\"regList[0].categoryId\" value=\"120901\">
        //<input type =\"hidden\" name=\"regList[0].packageId\" value=\"12A310\">
        //<input type =\"hidden\" name=\"regList[0].orgGeo\" value=\"310000310101\">
        //<input type =\"hidden\" name=\"regList[0].relationshipId\" value=\"12000100120014\">
        //<input type =\"hidden\" name=\"regList[0].examId\" value=\"201812001\">
        //<input type =\"hidden\" name=\"regList[0].examDate\" value=\"2018.03.31 下午 ~ 2018.04.01 下午\">
        //<input type =\"hidden\" name=\"regList[0].subjectId\" value=\"120901\">

        //<input type =\"hidden\" name=\"scoreId\" value=\"\">
        //<input type =\"hidden\" name=\"uId\" value=\"94b3bf6d002f4fe586e122ce93c0299d\">
        //<input type =\"hidden\" name=\"proId\" value=\"12\">
        //<input type =\"hidden\" name=\"nodeCode\" value=\"0101\">
        //<input type =\"hidden\" name=\"studentType\" value=\"08\">
        //<input type =\"hidden\" name=\"examId\" value=\"201812001\">
        //<input type =\"hidden\" name=\"orgGeo\" value=\"310000310101\">
        //<input type =\"hidden\" name=\"paymentType\" value=\"1\">
        //<input type =\"hidden\" name=\"packageId\" value=\"120901#\">
        //<input type =\"hidden\" name=\"relationshipId\" value=\"12000100120014\">
        //</tr>



        //examId	201812001
        //nodeCode	0101
        //orgGeo	310000310101
        //packageId	120901#
        //paymentType	1
        //proId	12
        //regList[0].categoryId	120901
        //regList[0].examDate	2018.03.31+下午+~+2018.04.01+下午
        //regList[0].examId	201812001
        //regList[0].orgGeo	310000310101
        //regList[0].packageId	12A310
        //regList[0].relationshipId	12000100120014
        //regList[0].subjectId	120901
        //relationshipId	12000100120014
        //scoreId
        //studentType	08
        //uId	94b3bf6d002f4fe586e122ce93c0299d


        //{
        //  "msg" : "报名未开始。",
        //  "success" : false
        //}

        #endregion
    }
}