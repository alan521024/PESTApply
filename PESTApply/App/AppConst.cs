using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubleX.Module;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 常量/静态/固定值
    /// </summary>
    public class AppConst : DictionaryCodes
    {
        #region 缓存Key

        /// <summary>
        /// 请求任务列表
        /// </summary>
        public const string CacheQuestList = "_quest";

        #endregion

        #region 数据字典Code

        public const string DictionaryTimes = "TIMES";
        public const string DictionaryArea = "AREA";
        public const string DictionaryOrganiz = "ORGANIZ";
        public const string DictionaryCategory = "CATEGORY";
        public const string DictionarySubject = "SUBJECT";
        public const string DictionaryPackage = "PACKAGE";

        #endregion

    }
}
