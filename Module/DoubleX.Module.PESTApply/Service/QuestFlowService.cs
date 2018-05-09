namespace DoubleX.Module.PESTApply
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using DoubleX.Resource.Culture;
    using DoubleX.Infrastructure.Utility;
    using DoubleX.Framework;

    /// <summary>
    /// 请求任务流程业务
    /// </summary>
    public class QuestFlowService : ApplicationService, IQuestFlowService
    {
        public QuestFlowService(IRepository<QuestFlowEntity> _repository)
        {
            repository = _repository;
        }

        protected readonly IRepository<QuestFlowEntity> repository;

        /// <summary>
        /// 获取所有请求任务流程
        /// </summary>
        /// <returns></returns>
        public List<QuestFlowEntity> GetAll()
        {
            return repository.Find();
        }
    }
}
