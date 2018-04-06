using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 应用程序缓存信息
    /// </summary>
    public class AppCache : ObservableObject
    {
        public AppCache()
        {
            Quests = AppHelper.GetQuests();
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        public List<QuestEntity> Quests
        {
            get { return quests; }
            set
            {
                quests = value;
                RaisePropertyChanged(() => Quests);
            }
        }
        private List<QuestEntity> quests;
    }
}
