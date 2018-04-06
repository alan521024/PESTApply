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
    public class QuestModuleConfigure : IModuleConfigure
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Title { get { return "英语考试申请"; } }

        /// <summary>
        /// 模块(标识)
        /// </summary>
        public string Name { get { return "PESTApply"; } }

        /// <summary>
        /// 模块空间
        /// </summary>
        public string Namespace { get { return "DoubleX.Module.PESTApply"; } }

        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assemblies { get { return this.GetType().Assembly; } }

        /// <summary>
        /// 是否插件
        /// </summary>
        public bool IsPlug { get { return false; } }

        /// <summary>
        /// 模块安装
        /// </summary>
        public void Install()
        {
            this.Configuration();
        }

        /// <summary>
        /// 模块卸载
        /// </summary>
        public void Shutdown()
        {

        }

        /// <summary>
        /// 模块配置
        /// </summary>
        public void Configuration()
        {
            //仓储/业务 IOC注入配置
            EngineHelper.RegisterType<IQuestService, QuestService>(DomainConfiguration.Options.ServiceIocRegisterOption);
            EngineHelper.RegisterType<IQuestFlowService, QuestFlowService>(DomainConfiguration.Options.ServiceIocRegisterOption);
            EngineHelper.RegisterType<IPETSPackageService, PETSPackageService>(DomainConfiguration.Options.ServiceIocRegisterOption);

        }
    }
}
