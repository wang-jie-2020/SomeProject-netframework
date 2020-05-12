using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Dto.Account
{
    /// <summary>
    /// 传输层示例 定义从核心业务层返回到应用层或展现层的数据类型
    /// </summary>
    public class UserInfo
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }
    }
}
