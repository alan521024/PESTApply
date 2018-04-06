namespace DoubleX.Module.PESTApply
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using DoubleX.Resource.Language;
    using DoubleX.Infrastructure.Utility;
    using DoubleX.Framework;

    /// <summary>
    /// 请求任务业务接口
    /// </summary>
    public interface IQuestService : IApplicationService
    {
        /// <summary>
        /// 获取所有请求任务
        /// </summary>
        /// <returns></returns>
         List<QuestEntity> GetAll();
    }
}
