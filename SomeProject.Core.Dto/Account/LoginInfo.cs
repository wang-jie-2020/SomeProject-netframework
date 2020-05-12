using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Dto.Account
{
    /// <summary>
    /// 传输层示例 定义从应用层或展现层传递到核心业务层的数据类型
    /// </summary>
    public class LoginInfo
    {
        public string LoginName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }
    }
}
