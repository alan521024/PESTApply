namespace DoubleX.Module.PESTApply
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using AutoMapper;
    using DoubleX.Resource.Language;
    using DoubleX.Infrastructure.Utility;
    using DoubleX.Framework;

    public class PESTApplyModuleMapperProfile : AutoMapper.Profile
    {
        public PESTApplyModuleMapperProfile()
        {
            //CreateMap<SigninInput, AccountEntity>();
            //CreateMap<AccountEntity, SigninOutput>();
        }
        
    }
}
