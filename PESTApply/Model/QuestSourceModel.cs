using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using GalaSoft.MvvmLight;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 请求数据源
    /// </summary>
    public class QuestSourceModel : ObservableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 考试信息(匹配成功后显示名称)
        /// </summary>
        public string Examination { get; set; }


        //*************值相关数据可能多个以'|'分割 


        /// <summary>
        /// Excel考试时间内容
        /// </summary>
        public string ExamText { get; set; }

        /// <summary>
        /// Excel地区内容
        /// </summary>
        public string AreaText { get; set; }

        /// <summary>
        /// Excel考场内容
        /// </summary>
        public string OrgText { get; set; }

        /// <summary>
        /// Excel项目内容(可能多个科目)
        /// </summary>
        public string CategoryText { get; set; }

        /// <summary>
        /// 考试时间值
        /// </summary>
        public string ExamValue { get; set; }

        /// <summary>
        /// 考试时间名称值
        /// </summary>
        public string ExamNameValue { get; set; }

        /// <summary>
        /// 地区值
        /// </summary>
        public string AreaValue { get; set; }

        /// <summary>
        /// 考场值
        /// </summary>
        public string RelationshipValue { get; set; }

        /// <summary>
        /// 项目值
        /// </summary>
        public string CategoryValue { get; set; }

        /// <summary>
        /// 科目值
        /// </summary>
        public string SubjectValue { get; set; }

        /// <summary>
        /// 考试值(报考系统Id值)
        /// </summary>
        public string PackageValue { get; set; }

        /// <summary>
        /// 考试数据(存储数据库Id值)
        /// </summary>
        public string PackageIdValue { get; set; }


        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }
        private string message;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryTotal
        {
            get { return retryTotal; }
            set
            {
                retryTotal = value;
                RaisePropertyChanged(() => RetryTotal);
            }
        }
        private int retryTotal;

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin
        {
            get { return isLogin; }
            set
            {
                isLogin = value;
                RaisePropertyChanged(() => IsLogin);
            }
        }
        private bool isLogin;

        /// <summary>
        /// 是否执行
        /// </summary>
        public bool IsWork
        {
            get { return isWork; }
            set
            {
                isWork = value;
                RaisePropertyChanged(() => isWork);
            }
        }
        private bool isWork;

        /// <summary>
        /// Cookies值
        /// </summary>
        public CookieCollection Cookies { get; set; }

    }
}