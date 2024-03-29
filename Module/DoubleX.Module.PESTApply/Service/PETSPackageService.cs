﻿namespace DoubleX.Module.PESTApply
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
    /// PEST考试信息业务
    /// </summary>
    public class PETSPackageService : ApplicationService, IPETSPackageService
    {
        public PETSPackageService(IRepository<PETSPackageEntity> _repository)
        {
            repository = _repository;
        }

        private readonly IRepository<PETSPackageEntity> repository;

        public List<PETSPackageEntity> GetAll()
        {
            return repository.Find();
        }

        public int BatchInsert(List<PETSPackageEntity> sources)
        {
            if (sources.IsEmpty())
                return 0;
            return repository.Insert(sources);
        }

        public int DeleteAll()
        {
            return repository.Delete(x => true);
        }

    }
}
