using SomeProject.Infrastructure.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;
using SomeProject.Core.Repository.Account.impl;
using System.Data.Entity;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Extensions;

namespace SomeProject.Console
{
    public class EfQueryTest
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

        //由于延迟加载导航属性，每次导航属性用到时都会打开数据库进行读取
        public void Method1()
        {
            using (var db = new DefaultDbContext())
            {
                var users = db.Set<SysUser>();
                foreach (var user in users)
                {
                    System.Console.WriteLine(user.Roles.Count());
                }
            }
        }

        //本架构下的同样
        public void Method2()
        {
            var repo = new UserRepository();
            var users = repo.Entities;
            foreach (var user in users)
            {
                System.Console.WriteLine(user.Roles.Count());
            }

        }

        //那么提前加载好导航属性是一个不错的方法
        public void Method3()
        {
            using (var db = new DefaultDbContext())
            {
                var users = db.Set<SysUser>().Include(p => p.Roles);
                foreach (var user in users)
                {
                    System.Console.WriteLine(user.Roles.Count());
                }
            }
        }

        //本架构下的同样
        public void Method4()
        {
            var repo = new UserRepository();
            var users = repo.Entities.Include(p => p.Roles);
            foreach (var user in users)
            {
                System.Console.WriteLine(user.Roles.Count());
            }
        }

        //但它仍旧不如DTO方式来的直接明了
        public void Method5()
        {
            using (var db = new DefaultDbContext())
            {
                var users = db.Set<SysUser>().Select(m => new
                {
                    roleCount = new { nCount = m.Roles.Count }
                });

                foreach (var user in users)
                {
                    System.Console.WriteLine(user.roleCount.nCount);
                }
            }
        }

        //本架构下的同样
        public void Method6()
        {
            var repo = new UserRepository();
            var users = repo.Entities.Select(m => new
            {
                roleCount = new { nCount = m.Roles.Count }
            });

            foreach (var user in users)
            {
                System.Console.WriteLine(user.roleCount.nCount);
            }
        }

        //架构中的查询Entity是不进行变更追踪的，注意区别
        public void Method7()
        {
            using (var db = new DefaultDbContext())
            {
                var user = db.Set<SysUser>().First();
                System.Console.WriteLine("查询时保留追踪：{0}", user.AddDate);

                user.AddDate = DateTime.Now.AddHours(10);
                db.SaveChanges();

                var newUser = db.Set<SysUser>().First();
                System.Console.WriteLine("查询时保留追踪：{0}", newUser.AddDate);
            }


            using (var db = new DefaultDbContext())
            {
                var user = db.Set<SysUser>().AsNoTracking().First();
                System.Console.WriteLine("查询时不保留追踪：{0}", user.AddDate);

                user.AddDate = DateTime.Now.AddHours(10);
                db.SaveChanges();

                var newUser = db.Set<SysUser>().First();
                System.Console.WriteLine("查询时不保留追踪：{0}", newUser.AddDate);
            }
        }

        //查询分页的封装
        public void Method8()
        {
            var repo = new UserRepository();
            PropertySort[] sortConditions = new[] { new PropertySort("AddDate", ListSortDirection.Descending), new PropertySort("UserName") };
            int total;

            var users = repo.Entities.Page<SysUser>(m => true, 1, 15, out total, sortConditions).ToList();
            foreach (var user in users)
            {
                System.Console.WriteLine(user.NickName);
            }
        }
    }
}
