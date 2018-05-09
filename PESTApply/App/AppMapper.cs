using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoubleX.Infrastructure.Utility;
using DoubleX.Framework;
using DoubleX.Module;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    public class AppMapper : AutoMapper.Profile
    {
        public AppMapper()
        {
            CreateMap<DictionaryModel, DictionaryEntity>();
            CreateMap<DictionaryEntity, DictionaryModel>();
            //CreateMap<AccountEntity, SigninOutput>();
        }
    }

}
