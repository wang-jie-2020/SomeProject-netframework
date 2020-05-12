using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Repository.Account.impl;
using SomeProject.Infrastructure.Common.Exceptions;

namespace SomeProject.Console
{
    public class EfOneToManyTest
    {
        #region 公共调用方法

        public static void RunTest()
        {
            while (true)
            {
                try
                {
                    System.Console.WriteLine("请输入命令：0; 退出程序，功能命令：1 - n");
                    string input = System.Console.ReadLine();
                    if (input == null)
                    {
                        continue;
                    }

                    if (input == "0")
                    {
                        break;
                    }

                    Type type = MethodBase.GetCurrentMethod().DeclaringType;
                    object o = Activator.CreateInstance(type);
                    type.InvokeMember("Method" + input, BindingFlags.Default | BindingFlags.InvokeMethod, null, o,
                        new object[] { });
                }
                catch (Exception e)
                {
                    ExceptionHandler(e);
                }
            }
        }

        private static void ExceptionHandler(Exception e)
        {
            ExceptionMessage emsg = new ExceptionMessage(e);
            System.Console.WriteLine(emsg.ErrorDetails);
        }

        #endregion

        //一步新增，从UserRepo
        public void Method1()
        {
            var repo = new UserRepository();
            var repo1 = new LoginLogRepository();

            var user = new SysUser()
            {
                UserName = "user1",
                Password = "user1",
                LoginLogs = new List<LoginLog>()
                {
                    new LoginLog() {IpAddress = "user1的ip"}
                }
            };

            repo.Insert(user);
        }

        //一步新增，从LoginLogRepo，虽然不太可能出现这种可能性
        //测试一下1-1关系时错误的情况，从表带主表未包含的对象；也是错误的，新增了一次主表记录
        public void Method2()
        {
            var repo = new UserRepository();
            var repo1 = new LoginLogRepository();

            var user = new SysUser()
            {
                UserName = "user2",
                Password = "user2",
            };

            repo.Insert(user);

            var newUser = repo.Entities.FirstOrDefault(p => p.UserName == "user2");


            var newLoginLog = new LoginLog()
            {
                IpAddress = "user2的ip",
                User = newUser
            };

            repo1.Insert(newLoginLog);
        }

        //所以还是一样的，追踪的对象才不会出现这种问题，因为它的修改会被上下文追踪
        public void Method3()
        {
            var repo = new UserRepository();
            var repo1 = new LoginLogRepository();

            var user = new SysUser()
            {
                UserName = "user3",
                Password = "user3",
            };

            repo.Insert(user);

            var newLoginLog = new LoginLog()
            {
                IpAddress = "user3的ip",
                User = user
            };

            repo1.Insert(newLoginLog);
        }

        //最正常的新增方式
        public void Method4()
        {
            var repo = new UserRepository();
            var repo1 = new LoginLogRepository();

            var user = new SysUser()
            {
                UserName = "user4",
                Password = "user4",
            };

            repo.Insert(user);

            var newUser = repo.Entities.FirstOrDefault(p => p.UserName == "user4");

            var newLoginLog = new LoginLog()
            {
                IpAddress = "user4的ip",
                UserId = newUser.Id
            };

            repo1.Insert(newLoginLog);
        }

        //两表同时修改
        public void Method5()
        {
            var repo = new UserRepository();
            var repo1 = new LoginLogRepository();

            var user = new SysUser()
            {
                UserName = "user5",
                Password = "user5",
                LoginLogs = new List<LoginLog>()
                {
                    new LoginLog() {IpAddress = "user5的ip"}
                }
            };
            repo.Insert(user);

            SysUser newUser = repo.Entities.Include(p => p.LoginLogs).SingleOrDefault(o => o.UserName == "user5");
            newUser.UserName = "修改的user5";
            newUser.LoginLogs.FirstOrDefault().IpAddress = "修改的user5的ip";

            repo.Update(newUser, false);
            repo1.Update(newUser.LoginLogs.FirstOrDefault(), false);
            repo.Save();
        }

        //其他的就不测试了
    }
}
