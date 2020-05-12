using SomeProject.Core.Dto.Account;
using SomeProject.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;

namespace SomeProject.Core.Domain.Account
{
    /// <summary>
    /// 核心业务层示例 处理与交互无关的核心业务
    /// </summary>
    public interface IAccountService
    {
        #region 属性

        IQueryable<SysUser> Users { get; }

        IQueryable<SysUserExpand> UserExpands { get; }

        IQueryable<LoginLog> LoginLogs { get; }

        IQueryable<SysRole> Roles { get; }

        IQueryable<SysDepartment> Departments { get; }

        #endregion

        #region 方法

        /// <summary>
        ///     用户登录
        /// </summary>
        /// <param name="loginInfo">登录信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult Login(LoginInfo loginInfo);

        #endregion

    }
}
