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
    /// 请求任务业务
    /// </summary>
    public class QuestService : ApplicationService, IQuestService
    {
        public QuestService(IRepository<QuestEntity> _repository)
        {
            repository = _repository;
        }

        protected readonly IRepository<QuestEntity> repository;

        /// <summary>
        /// 获取所有请求任务
        /// </summary>
        /// <returns></returns>
        public List<QuestEntity> GetAll()
        {
            return repository.Query();
        }
    }
}
