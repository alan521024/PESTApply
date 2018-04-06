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
    /// PETS考试信息实体
    /// </summary>
    [SugarTable("QST_PETSPackage")]
    public class PETSPackageEntity : BaseEntity
    {
        public string ProId { get; set; }
        public string OrgGeo { get; set; }
        public string RelationshipId { get; set; }
        public string OrgName { get; set; }
        public string ExamId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string FullName { get; set; }
        public string ExamDate { get; set; }
        public bool Capacity { get; set; }
        public string Price { get; set; }
        public string StudentType { get; set; }
        public string Status { get; set; }
        public long RegStartDate { get; set; }
        public long RegEndDate { get; set; }
        public long RunEndDate { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string GoodsType { get; set; }
        public string StartNum { get; set; }
        public string EnNum { get; set; }
        public string PackageId { get; set; }

    }
}
