using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;
using SomeProject.Core.Repository.Account.impl;
using SomeProject.Infrastructure.Common.Exceptions;

namespace SomeProject.Console
{
    public class EfManyToManyTest
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

        //直接进入互相关联的测试，不考虑同步增加的情况
        //还是按照对象修改的方式，很不出意外的失败了
        public void Method1()
        {
            var repo = new UserRepository();
            var repo1 = new RoleRepository();

            var users = new List<SysUser>()
            {
                new SysUser()
                {
                    UserName = "user1",
                    Password = "user1"
                }
            };
            repo.Insert(users);

            var roles = new List<SysRole>()
            {
                new SysRole()
                {
                    Name = "管理员",
                    Description = "管理员",
                    RoleType = RoleType.Admin
                },
                new SysRole()
                {
                    Name = "游客",
                    Description = "游客",
                    RoleType = RoleType.Guest
                }
            };
            repo1.Insert(roles);

            var newUser = repo.Entities.FirstOrDefault(o => o.UserName == "user1");
            var newRole = repo1.Entities.FirstOrDefault(o => o.Name == "管理员");

            newUser.NickName = "修改的昵称";
            newUser.Roles = new List<SysRole>() { newRole };

            repo.Update(newUser);
        }

        //没有id，只想到带入到上下文中进行修改，是可行的
        //非常不方便，所以更好的方案应该是做两个1-多关系
        public void Method2()
        {
            var repo = new UserRepository();
            var repo1 = new RoleRepository();

            var users = new List<SysUser>()
            {
                new SysUser()
                {
                    UserName = "user2",
                    Password = "user2"
                }
            };
            repo.Insert(users);

            var roles = new List<SysRole>()
            {
                new SysRole()
                {
                    Name = "管理员2",
                    Description = "管理员2",
                    RoleType = RoleType.Admin
                }
            };
            repo1.Insert(roles);

            long userId = repo.Entities.FirstOrDefault(o => o.UserName == "user2").Id;
            long roleId = repo1.Entities.FirstOrDefault(o => o.Name == "管理员3").Id;

            var user = repo.Query(userId);
            var role = repo1.Query(roleId);

            user.Roles = new List<SysRole>() { role };

            repo.Update(user);
        }

        //删除测试，还是对象
        public void Method3()
        {
            var repo = new UserRepository();
            var repo1 = new RoleRepository();

            var users = new List<SysUser>()
            {
                new SysUser()
                {
                    UserName = "user3",
                    Password = "user3"
                }
            };
            repo.Insert(users);

            var roles = new List<SysRole>()
            {
                new SysRole()
                {
                    Name = "管理员3",
                    Description = "管理员3",
                    RoleType = RoleType.Admin
                }
            };
            repo1.Insert(roles);

            long userId = repo.Entities.FirstOrDefault(o => o.UserName == "user3").Id;
            long roleId = repo1.Entities.FirstOrDefault(o => o.Name == "管理员3").Id;

            var user = repo.Query(userId);
            var role = repo1.Query(roleId);
            user.Roles = new List<SysRole>() { role };
            repo.Update(user);

            user.Roles = null;
            repo.Update(user);
        }
    }
}
