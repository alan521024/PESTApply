using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using GalaSoft.MvvmLight;
using DoubleX.Infrastructure.Utility;
using DoubleX.Infrastructure.DesktopUI;
using DoubleX.Framework;
using DoubleX.Module.Basics;
using DoubleX.Module.PESTApply;

namespace PESTApply
{
    /// <summary>
    /// 数据字典信息
    /// </summary>
    public class DictionaryModel : ObservableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 类型编码
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 父值
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// 关联值
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }
    }
}
