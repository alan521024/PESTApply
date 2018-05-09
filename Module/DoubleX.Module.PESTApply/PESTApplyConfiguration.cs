using System;
using System.Reflection;
using System.Collections.Generic;
using DoubleX.Infrastructure.Utility;
using DoubleX.Framework;
using DoubleX.Module.PESTApply;
using FluentValidation;

namespace DoubleX.Module.PESTApply
{
    /// <summary>
    /// 请求任务模块配置
    /// </summary>
    public class PESTApplyConfiguration : IComponentConfiguration
    {
        /// <summary>
        /// 组件名称(命名空间)
        /// </summary>
        public string Namespace { get { return "DoubleX.Module.PESTApply"; } }

        /// <summary>
        /// 组件标识
        /// </summary>
        public string Name { get { return "PESTApply"; } }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Title { get { return "英语考试申请"; } }

        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assemblies { get { return this.GetType().Assembly; } }

        /// <summary>
        /// 组件安装
        /// </summary>
        public void Install()
        {
            this.Configuration();
        }

        /// <summary>
        /// 组件卸载
        /// </summary>
        public void Shutdown()
        {

        }

        /// <summary>
        /// 组件配置
        /// </summary>
        public void Configuration()
        {
            //仓储/业务 IOC注入配置
            EngineHelper.RegisterType<IQuestService, QuestService>(DomainConfiguration.Options.IocServiceOption);
            EngineHelper.RegisterType<IQuestFlowService, QuestFlowService>(DomainConfiguration.Options.IocServiceOption);
            EngineHelper.RegisterType<IPETSPackageService, PETSPackageService>(DomainConfiguration.Options.IocServiceOption);

        }
    }
}
