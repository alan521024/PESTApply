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
    /// PEST考试信息业务接口
    /// </summary>
    public interface IPETSPackageService : IApplicationService
    {
        /// <summary>
        /// 获取所有考试信息内容
        /// </summary>
        /// <returns></returns>
        List<PETSPackageEntity> GetAll();

        /// <summary>
        /// 批量增加考试信息
        /// </summary>
        int BatchInsert(List<PETSPackageEntity> sources);

        /// <summary>
        /// 删除考试信息
        /// </summary>
        int DeleteAll();
    }
}
