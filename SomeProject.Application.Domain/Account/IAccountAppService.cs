using SomeProject.Application.ViewModel.Account;
using SomeProject.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Application.Domain.Account
{
    public interface IAccountAppService
    {
        /// <summary>
        ///  用户登录
        /// </summary>
        /// <param name="model">登录模型信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult Login(LoginViewModel model);

        /// <summary>
        /// 用户退出
        /// </summary>
        void Logout();
    }
}
