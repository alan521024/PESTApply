namespace DoubleX.Module.PESTApply
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SqlSugar;
    using DoubleX.Resource.Language;
    using DoubleX.Infrastructure.Utility;
    using DoubleX.Framework;

    /// <summary>
    /// 请求任务信息实体
    /// </summary>
    [SugarTable("QST_Quest")]
    public class QuestEntity : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 流程列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<QuestFlowEntity> Flows { get; set; }
        
    }
}
