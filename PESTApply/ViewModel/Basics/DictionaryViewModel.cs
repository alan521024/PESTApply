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
using DoubleX.Module;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    public class DictionaryViewModel : ViewModelDefault
    {
        protected IDictionaryService dictionaryService;
        protected IPETSPackageService petsPackageService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryViewModel()
        {
            dictionaryService = EngineHelper.Resolve<IDictionaryService>();
            petsPackageService = EngineHelper.Resolve<IPETSPackageService>();

            var dictionarys = dictionaryService.GetAll().OrderBy(x => x.Genre).ToList();
            SourceList = new ObservableCollection<DictionaryModel>(EngineHelper.Map<List<DictionaryModel>>(dictionarys));
            CaptchaBitmapImage = GetCaptcha();
        }

        #region 公共属性

        /// <summary>
        /// 操作数据源
        /// </summary>
        public ObservableCollection<DictionaryModel> SourceList
        {
            get { return sourceList; }
            set { sourceList = value; RaisePropertyChanged(() => SourceList); }
        }
        private ObservableCollection<DictionaryModel> sourceList = new ObservableCollection<DictionaryModel>();

        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(() => Account); }
        }
        private string account = AppHelper.ValueAccount;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(() => Password); }
        }
        private string password = AppHelper.ValuePassword;

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

        #region Event 同步数据

        public RelayCommand OnSync
        {
            get
            {
                if (onSync == null)
                {
                    return new RelayCommand(() =>
                    {
                        var task = Task.Run(() =>
                        {
                            ShowStatusText("开始同步，正在登录", isConsoleWrite: false);
                            var result = AppHelper.GetLoginResult(Account, Password, CaptchaCode, CurrentCookies,
                                (res, model) =>
                                {
                                    //DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    //{
                                    //    CaptchaBitmapImage = GetCaptcha(); //重置了Cookies,登录失效
                                    //});
                                    CaptchaCode = "****";
                                    CurrentCookies = res.CookieCollection;
                                    ShowStatusText(string.Format("{0},开始同步.", model.msg), isConsoleWrite: false);
                                    try
                                    {
                                        var list = SyncRemote().OrderBy(x => x.Genre).ToList();
                                        SourceList = new ObservableCollection<DictionaryModel>(list);
                                    }
                                    catch (Exception ex)
                                    {
                                        AppHelper.ShowMsg(ExceptionHelper.GetMessage(ex), icon: System.Windows.MessageBoxImage.Error);
                                    }
                                },
                                (res, model) =>
                                {
                                    ShowStatusText(model.msg, isConsoleWrite: false);
                                    AppHelper.ShowMsg(model.msg, icon: System.Windows.MessageBoxImage.Error);
                                });

                            ShowStatusText("同步结束", isConsoleWrite: false);
                        });
                    });
                }
                return onSync;
            }
            set { onSync = value; }
        }
        private RelayCommand onSync;

        #endregion

        #region 辅助操作

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

        /// <summary>
        /// 远程数据同步
        /// </summary>
        private List<DictionaryModel> SyncRemote()
        {
            List<DictionaryModel> source = new List<DictionaryModel>();

            ShowStatusText("正在同步考次(时间)信息..", isConsoleWrite: false);
            var times = SysTimes();
            if (!VerifyHelper.IsEmpty(times))
            {
                source.AddRange(times);
            }

            ShowStatusText("正在同步城区(上海)信息..", isConsoleWrite: false);
            var areas = SyncArea();
            if (!VerifyHelper.IsEmpty(areas))
            {
                source.AddRange(areas);
            }

            ShowStatusText("正在同步考场/科目信息..", isConsoleWrite: false);
            var terminals = SyncOrgAndCategory(areas);
            if (!VerifyHelper.IsEmpty(terminals))
            {
                source.AddRange(terminals);
            }

            ShowStatusText("正在获取考试信息..", isConsoleWrite: false);
            List<DictionaryModel> subjects = new List<DictionaryModel>();
            var packages = SyncPackagesAndSubject(source, out subjects);

            if (!VerifyHelper.IsEmpty(subjects))
            {
                source.AddRange(subjects);
            }
            if (!VerifyHelper.IsEmpty(packages))
            {
                packages.ForEach(x =>
                {
                    source.Add(new DictionaryModel()
                    {
                        Id = Guid.NewGuid(),
                        Genre = AppConst.DictionaryPackage,
                        Name = string.Format("【{0}】{1}/{2}/{3}", x.ExamDate, x.FullName, x.OrgName, x.CategoryName),
                        Value = string.Format("{0}|{1}", x.CategoryId, x.SubjectId),
                        Relation = JsonHelper.Serialize(x),
                        Parent = "",
                        Sort = 0
                    });
                });
            }


            SavePETSPackage(packages);

            SaveDictionarys(source);

            return source;
        }

        /// <summary>
        /// 同步考次(考试时间)
        /// </summary>
        /// <returns></returns>
        private List<DictionaryModel> SysTimes()
        {
            List<DictionaryModel> datas = new List<DictionaryModel>();

            var options = new HttpClientOptions();
            options.URL = AppHelper.UrlApplyPage;
            options.Method = "GET";
            options.CookieCollection = CurrentCookies;
            var result = new HttpWebClientUtility().Request(options);
            if (!VerifyHelper.IsEmpty(result.Content))
            {
                var doc = HtmlDocumentHelper.Load(result.Content);
                if (doc != null)
                {
                    var timesNode = HtmlDocumentHelper.FindChildNodes(doc, AppHelper.XPathTimes);
                    if (timesNode != null)
                    {
                        foreach (var item in timesNode.Where(x => x.OriginalName == "a"))
                        {
                            var valueAttribute = item.Attributes["attrval"];
                            if (valueAttribute != null)
                            {
                                datas.Add(new DictionaryModel()
                                {
                                    Id = Guid.NewGuid(),
                                    Genre = AppConst.DictionaryTimes,
                                    Name = StringHelper.Get(item.InnerText),
                                    Value = StringHelper.Get(valueAttribute.Value),
                                    Parent = "",
                                    Sort = 0
                                });
                            }
                        }
                    }
                }
            }

            return datas;
        }

        /// <summary>
        /// 同步地址
        /// </summary>
        private List<DictionaryModel> SyncArea()
        {
            List<DictionaryModel> source = new List<DictionaryModel>();
            var options = new HttpClientOptions();
            options.URL = AppHelper.UrlAreas;
            options.Method = "GET";
            options.CookieCollection = CurrentCookies;
            var result = new HttpWebClientUtility().Request(options);
            if (result != null && !VerifyHelper.IsEmpty(result.Content))
            {
                var obj = JsonHelper.Deserialize<JObject>(result.Content);
                if (BoolHelper.Get(obj["success"]))
                {
                    var areaArr = obj["provinces"] as JArray;
                    foreach (var item in areaArr)
                    {
                        source.Add(new DictionaryModel()
                        {
                            Id = Guid.NewGuid(),
                            Genre = AppConst.DictionaryArea,
                            Name = StringHelper.Get(item["name"]),
                            Value = StringHelper.Get(item["fullId"]),
                            Parent = StringHelper.Get(item["pid"]),
                            Sort = 0
                        });
                    }
                }
            }
            return source;
        }

        /// <summary>
        /// 同步考场，科目
        /// </summary>
        private List<DictionaryModel> SyncOrgAndCategory(List<DictionaryModel> areas)
        {
            List<DictionaryModel> datas = new List<DictionaryModel>();

            foreach (var areaItem in areas)
            {
                var options = new HttpClientOptions();
                options.URL = AppHelper.UrlOrgAndSubject;
                options.Method = "POST";
                options.ContentType = "application/x-www-form-urlencoded";
                options.Accept = "*/*";
                options.CookieCollection = CurrentCookies;
                var loginParam = new List<KeyValueModel>();
                loginParam.Add(new KeyValueModel("category", ""));
                loginParam.Add(new KeyValueModel("geoId", areaItem.Value));
                loginParam.Add(new KeyValueModel("proId", AppHelper.ValueProId));
                loginParam.Add(new KeyValueModel("studentType", AppHelper.ValueStudentType));
                loginParam.Add(new KeyValueModel("subjectId", "*"));
                loginParam.Add(new KeyValueModel("timeId", "*"));
                options.PostData = StringHelper.GetFormStringByKeyValues(loginParam);
                var result = new HttpWebClientUtility().Request(options);
                if (result != null && !VerifyHelper.IsEmpty(result.Content))
                {
                    var obj = JsonHelper.Deserialize<JObject>(result.Content);
                    if (BoolHelper.Get(obj["success"]))
                    {
                        var orgArr = obj["orglist"] as JArray;
                        foreach (var item in orgArr)
                        {
                            if (datas.Where(x => x.Genre == AppConst.DictionaryOrganiz && x.Value == StringHelper.Get(item["fullId"])).Count() == 0)
                            {
                                datas.Add(new DictionaryModel()
                                {
                                    Id = Guid.NewGuid(),
                                    Genre = AppConst.DictionaryOrganiz,
                                    Name = StringHelper.Get(item["exaOrgName"]),
                                    Value = StringHelper.Get(item["relationshipId"]),
                                    Relation = areaItem.Value,
                                    Parent = "",
                                    Sort = 0
                                });
                            }
                        }
                        var packageArr = obj["packageList"] as JArray;
                        foreach (var item in packageArr)
                        {
                            if (datas.Where(x => x.Genre == AppConst.DictionaryCategory && x.Value == StringHelper.Get(item["uniteId"])).Count() == 0)
                            {
                                datas.Add(new DictionaryModel()
                                {
                                    Id = Guid.NewGuid(),
                                    Genre = AppConst.DictionaryCategory,
                                    Name = StringHelper.Get(item["packageName"]),
                                    Value = StringHelper.Get(item["uniteId"]),
                                    Parent = "",
                                    Sort = 0
                                });
                            }

                        }
                    }
                }
            }

            return datas;
        }

        /// <summary>
        /// 同步考试
        /// </summary>
        private List<PETSPackageEntity> SyncPackagesAndSubject(List<DictionaryModel> dictionarys, out List<DictionaryModel> subjects)
        {
            var subjectSource = new List<DictionaryModel>();
            var areas = dictionarys.Where(x => x.Genre == AppConst.DictionaryArea).ToList();
            var orgs = dictionarys.Where(x => x.Genre == AppConst.DictionaryOrganiz).ToList();
            var categorys = dictionarys.Where(x => x.Genre == AppConst.DictionaryCategory).ToList();
            var times = dictionarys.Where(x => x.Genre == AppConst.DictionaryTimes).ToList();

            List<PETSPackageEntity> source = new List<PETSPackageEntity>();

            if (!VerifyHelper.IsEmpty(areas) && !VerifyHelper.IsEmpty(orgs) && !VerifyHelper.IsEmpty(categorys) && !VerifyHelper.IsEmpty(times))
            {
                foreach (var time in times)
                {
                    foreach (var area in areas)
                    {
                        foreach (var org in orgs.Where(x => x.Relation == area.Value))
                        {
                            foreach (var category in categorys)
                            {
                                ShowStatusText(string.Format("正在获取考试信息({0} / {1} / {2} / {3})..", time.Name, area.Name, org.Name, category.Name), isConsoleWrite: false);

                                var options = new HttpClientOptions();
                                options.URL = AppHelper.UrlPackage + string.Format("&proId={0}&geoId={1}&orgId={2}&projectId={3},&timeId={4}&categoryId=&studentType={5}&onid=projectType&_={6}",
                                    AppHelper.ValueProId, area.Value, org.Value, category.Value, time.Value, AppHelper.ValueStudentType, DateTimeHelper.GetToUnixTimestamp());
                                options.Method = "GET";
                                options.CookieCollection = CurrentCookies;
                                var result = new HttpWebClientUtility().Request(options);
                                if (result != null && !VerifyHelper.IsEmpty(result.Content))
                                {
                                    var obj = JsonHelper.Deserialize<JObject>(result.Content);
                                    var arr = obj["regExamInfolist"] as JArray;
                                    if (arr != null && arr.Count() > 0)
                                    {
                                        if (BoolHelper.Get(obj["success"]) && obj["regExamInfolist"][0]["regExamInfos"] != null)
                                        {
                                            JArray examArr = obj["regExamInfolist"][0]["regExamInfos"] as JArray;
                                            if (!examArr.IsEmpty())
                                            {
                                                for (var i = 0; i < examArr.Count; i++)
                                                {
                                                    if (StringHelper.Get((examArr[i] as JObject).GetString("id")).IsEmpty())
                                                    {
                                                        examArr[i]["id"] = Guid.NewGuid().ToString();
                                                    }
                                                }
                                            }

                                            var packages = JsonHelper.Deserialize<List<PETSPackageEntity>>(StringHelper.Get(obj["regExamInfolist"][0]["regExamInfos"]));
                                            packages.ForEach(item =>
                                            {
                                                //subject
                                                if (subjectSource.Where(x => x.Value == item.SubjectId).Count() == 0)
                                                {
                                                    subjectSource.Add(new DictionaryModel()
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        Genre = AppConst.DictionarySubject,
                                                        Name = item.SubjectName,
                                                        Value = item.SubjectId,
                                                        Parent = "",
                                                        Sort = 0
                                                    });
                                                }

                                                //package
                                                if (source.Where(x => x.ExamId == item.ExamId && x.OrgGeo == item.OrgGeo && x.RelationshipId == item.RelationshipId && x.CategoryId == item.CategoryId && x.SubjectId == item.SubjectId).Count() == 0)
                                                {
                                                    item.Id = Guid.NewGuid();
                                                    source.Add(item);
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }
            }

            subjects = subjectSource;
            return source;
        }

        /// <summary>
        /// 保存数据字典
        /// </summary>
        /// <param name="source"></param>
        private void SaveDictionarys(List<DictionaryModel> source)
        {
            if (VerifyHelper.IsEmpty(source))
                return;

            ShowStatusText("正在保存数据字典信..", isConsoleWrite: false);

            var datas = EngineHelper.Map<List<DictionaryEntity>>(source);

            var genres = datas.GroupBy(x => x.Genre).Select(x => x.Key).ToArray();
            dictionaryService.DeleteByGenres(genres);
            dictionaryService.BatchInsert(datas);
        }

        /// <summary>
        /// 保存PEST考试信息
        /// </summary>
        /// <param name="source"></param>
        private void SavePETSPackage(List<PETSPackageEntity> source)
        {
            if (VerifyHelper.IsEmpty(source))
                return;

            ShowStatusText("正在保存PEST考试信息..", isConsoleWrite: false);

            petsPackageService.DeleteAll();
            petsPackageService.BatchInsert(source);
        }

        #endregion

    }
}