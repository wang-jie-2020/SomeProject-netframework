using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Dto.Account;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Helper;
using SomeProject.Core.Repository.Account;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;
using SomeProject.Infrastructure.Common.Extensions;

namespace SomeProject.Core.Domain.Account.impl
{
    public class AccountService : IAccountService
    {
        #region Ioc注入

        public IUserRepository UserRepository { get; set; }

        public IUserExpandRepository UserExpandRepository { get; set; }

        public ILoginLogRepository LoginLogRepository { get; set; }

        public IRoleRepository RoleRepository { get; set; }

        public IDepartmentRepository DepartmentRepository { get; set; }

        #endregion

        #region 属性

        public IQueryable<SysUser> Users
        {
            get { return UserRepository.Entities; }
        }

        public IQueryable<SysUserExpand> UserExpands
        {
            get { return UserExpandRepository.Entities; }
        }

        public IQueryable<LoginLog> LoginLogs
        {
            get { return LoginLogRepository.Entities; }
        }

        public IQueryable<SysRole> Roles
        {
            get { return RoleRepository.Entities; }
        }

        public IQueryable<SysDepartment> Departments
        {
            get { return DepartmentRepository.Entities; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginInfo">登录信息</param>
        /// <returns>业务操作结果</returns>
        public OperationResult Login(LoginInfo loginInfo)
        {
            PublicHelper.CheckArgument(loginInfo, "loginInfo");

            var user = UserRepository.Entities.SingleOrDefault(o => o.UserName == loginInfo.LoginName);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "指定账号的用户不存在。");
            }

            if (!(user.Password.Compare(loginInfo.Password)))
            {
                return new OperationResult(OperationResultType.Warning, "登录密码不正确。");
            }

            LoginLog loginLog = new LoginLog { IpAddress = loginInfo.IpAddress, UserId = user.Id };
            LoginLogRepository.Insert(loginLog);

            var info = new UserInfo()
            {
                UserName = user.UserName,
                Password = user.Password,
                NickName = user.NickName,
                Email = user.Email
            };

            return new OperationResult(OperationResultType.Success, "登录成功。", info);
        }

        #endregion

    }
}
