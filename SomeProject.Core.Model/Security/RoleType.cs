using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Model.Security
{
    /// <summary>
    /// 角色类型枚举
    /// </summary>
    [Description("角色类型")]
    public enum RoleType
    {
        [Description("游客")]
        Guest,

        [Description("低权限")]
        Low,

        [Description("高权限")]
        High,

        [Description("管理员")]
        Admin
    }
}
