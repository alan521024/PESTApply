using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DoubleX.Infrastructure.Utility;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 应用程序宿主服务
    /// </summary>
    public class AppHosting : AbsHosting, IHosting
    {
        public override Guid Key { get; }

        public override Action<object> OnStart { get; }

        public override Action<object> OnStop { get; }

        public override Action<object> OnBegin { get; }

        public override Action<object> OnEnd { get; }


        public AppHosting() : this(Guid.NewGuid(), null, null, null, null)
        {
            OnStart = (obj) => { };
            OnStop = (obj) => { };
            OnBegin = (obj) => { Begin(obj); };
            OnEnd = (obj) => { End(obj); };
        }

        public AppHosting(Action<object> start, Action<object> stop, Action<object> begin, Action<object> end) :
            this(Guid.NewGuid(), start, stop, begin, end)
        {

        }

        public AppHosting(Guid key, Action<object> start, Action<object> stop, Action<object> begin, Action<object> end)
        {
            Key = Guid.NewGuid();
            OnStart = start;
            OnStop = stop;
            OnBegin = begin;
            OnEnd = end;
        }

        public override void Initialize()
        {
            //(0)应用运行初始配置
            LoggingManager.AddLoggerAdapter(new Log4netLoggerAdapter());  //增加日志组件
            EngineHelper.LoggingInfo("Quest Application  - Start - ");

            //(1)领域相关初始配置
            DomainConfiguration.Initialize(opt =>
            {
                opt.RepositoryEntityKeyType = typeof(SqlSugarRepository<,>);
                opt.RepositoryEntityType = typeof(SqlSugarRepository<>);
            });

            //(2)模块处理
            EngineHelper.GetModuleAll();

            //(3)模块处理
            Mapper.Initialize(mapOpt =>
            {
                var profiles = EngineHelper.GetTypeFinder().FindClassesOfType<AutoMapper.Profile>();
                foreach (var item in profiles)
                {
                    mapOpt.AddProfile(item);
                }
            });

            //(End)构建容器
            EngineHelper.ContainerBuilder<IContainer>();
        }

        private void Begin(object obj)
        {
        }

        private void End(object obj)
        {
        }
    }
}
