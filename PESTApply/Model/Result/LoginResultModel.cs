using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PESTApply
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResultModel
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public dynamic obj { get; set; }
        public string token { get; set; }

    }
}
