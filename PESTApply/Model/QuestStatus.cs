using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PESTApply
{
    public enum QuestStatus
    {
        未操作 = 0,
        登录失败 = -1,
        登录失效 = -2,
        操作失败 = -3,
        登录成功 = 1,
        操作成功 = 2,
    }
}
