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
    /// 请求任务流程实体
    /// </summary>
    [SugarTable("QST_QuestFlow")]
    public class QuestFlowEntity : BaseEntity
    {
        /// <summary>
        /// 请求任务Id
        /// </summary>
        public Guid QuestId { get; set; }

        /// <summary>
        /// 流程描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求参数格式([{Name:"",Key:"",Value:""},....])
        /// </summary>
        public string ParamFormat { get; set; }

        /// <summary>
        /// 请求结果格式([{Name:"",Key:"",Value:""},....])
        /// </summary>
        public string ResultFormat { get; set; }

        /// <summary>
        /// 发生错误时标识({Key:"",Value:"",Type:"in/notin"}),in(含),notin(不含)
        /// </summary>
        public string ErrorTagFormat { get; set; }

        /// <summary>
        /// 发生错误时是否继续
        /// </summary>
        public Boolean IsErrorCarryon { get; set; }
    }
}
