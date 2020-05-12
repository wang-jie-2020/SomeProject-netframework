using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Application.ViewModel.Account;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Helper;
using SomeProject.Core.Dto.Account;
using System.Web;
using System.Web.Security;
using SomeProject.Core.Domain.Account;

namespace SomeProject.Application.Domain.Account.impl
{
    public class AccountAppService : IAccountAppService
    {
        #region 属性

        /// <summary>
        /// 获取或设置 用户信息数据访问对象
        /// </summary>
        public IAccountService AccountService { get; set; }

        #endregion

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model">登录模型信息</param>
        /// <returns>业务操作结果</returns>
        public OperationResult Login(LoginViewModel model)
        {
            PublicHelper.CheckArgument(model, "model");
            LoginInfo loginInfo = new LoginInfo
            {
                LoginName = model.LoginName,
                Password = model.Password,
                IpAddress = HttpContext.Current.Request.UserHostAddress
            };
            OperationResult result = AccountService.Login(loginInfo);
            if (result.ResultType == OperationResultType.Success)
            {
                var user = (UserInfo)result.AppendData;
                DateTime expiration = model.IsRememberLogin
                    ? DateTime.Now.AddDays(7)
                    : DateTime.Now.Add(FormsAuthentication.Timeout);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, expiration,
                    true, user.NickName, FormsAuthentication.FormsCookiePath);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                if (model.IsRememberLogin)
                {
                    cookie.Expires = DateTime.Now.AddDays(7);
                }
                HttpContext.Current.Response.Cookies.Set(cookie);
                result.AppendData = null;
            }
            return result;
        }

        /// <summary>
        ///     用户退出
        /// </summary>
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}
