using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using SomeProject.Core.Domain.Account;
using SomeProject.Core.Dto.Account;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Repository.Account.impl;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Extensions;
using SomeProject.Infrastructure.Ioc;

namespace SomeProject.Console
{
    public class AutoFacTest
    {
        #region 此框架中的登录测试

        static AutoFacTest()
        {
            //初始化容器，将需要用到的组件添加到容器中
            Container.Init();
        }

        /// <summary>
        /// 注意这里不会进行自动注入，控制台的解决未考虑，若是winform中可以将form进行统一处理
        /// </summary>
        public IAccountService AccountService { get; set; }

        public void Login()
        {
            //准备数据
            var userRepo = new UserRepository();
            var user = new SysUser()
            {
                UserName = "admin",
                Password = "admin",
            };
            userRepo.Insert(user);

            using (var scope = Container.Instance.BeginLifetimeScope())
            {
                AccountService = scope.Resolve<IAccountService>();

                OperationResult result = AccountService.Login(new LoginInfo
                {
                    LoginName = "admin",
                    Password = "admin",
                    IpAddress = "127.0.0.1"
                });

                System.Console.WriteLine(result.Message ?? result.ResultType.ToDescription());
                System.Console.ReadKey();
            }
        }

        #endregion
    }
}
