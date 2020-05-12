using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model;
using SomeProject.Core.Model.Account;
using SomeProject.Infrastructure.Common.Exceptions;
using SomeProject.Infrastructure.Data.Extensions;

namespace SomeProject.Console
{
    public class EfUpdateTest
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

        //准备一些查询数据
        static EfUpdateTest()
        {
            using (var context = new DefaultDbContext())
            {
                var user = context.Set<SysUser>().SingleOrDefault(o => o.UserName == "张三");
                if (user == null)
                {
                    context.Set<SysUser>().Add(new SysUser() { UserName = "张三", Password = "张三密码" });
                    context.Set<SysUser>().Add(new SysUser() { UserName = "李四", Password = "李四密码" });
                    context.Set<SysUser>().Add(new SysUser() { UserName = "王五", Password = "王五密码" });

                    context.SaveChanges();
                }
            }
        }

        //情景一：在同一个上下文中查询并修改
        public void Method1()
        {
            const string userName = "张三";

            using (var db = new DefaultDbContext())
            {
                SysUser oldUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新前：{0}。", oldUser.AddDate);

                oldUser.AddDate = oldUser.AddDate.AddMinutes(10);
                int count = db.SaveChanges();
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新后：{0}。", newUser.AddDate);
            }
        }

        //情景二：在一个上下文中查询，在另一个上下文中修改
        public void Method2()
        {
            const string userName = "张三";

            SysUser user;
            using (var db = new DefaultDbContext())
            {
                user = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新前：{0}。", user.AddDate);
            }
            user.AddDate = DateTime.Now;

            using (var db = new DefaultDbContext())
            {
                DbEntityEntry<SysUser> entry = db.Entry(user);

                System.Console.WriteLine("Attach前的状态：{0}", entry.State);//EntityState.Detached
                db.Set<SysUser>().Attach(user);
                System.Console.WriteLine("Attach后的状态：{0}", entry.State); //EntityState.Unchanged

                entry.State = EntityState.Modified;
                int count = db.SaveChanges();
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新后：{0}。", newUser.AddDate);
            }
        }

        //情景二的异常情况：上下文2中已存在与外来实体主键相同的数据了，这将引发InvalidOperationException异常
        public void Method3()
        {
            const string userName = "张三";

            SysUser user;
            using (var db = new DefaultDbContext())
            {
                user = db.Set<SysUser>().Single(m => m.UserName == userName);
            }
            user.AddDate = DateTime.Now;

            using (var db = new DefaultDbContext())
            {
                //通过查询，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Find(user.Id);
                System.Console.WriteLine("更新前：{0}。", oldUser.AddDate);

                DbEntityEntry<SysUser> entry = db.Entry(user);
                System.Console.WriteLine("Attach前的状态：{0}", entry.State);
                db.Set<SysUser>().Attach(user); //报错！！
                System.Console.WriteLine("Attach后的状态：{0}", entry.State);

                entry.State = EntityState.Modified;
                int count = db.SaveChanges();
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新后：{0}。", newUser.AddDate);
            }
        }

        //针对情景二的异常的处理方案
        public void Method4()
        {
            const string userName = "张三";

            SysUser user;
            using (var db = new DefaultDbContext())
            {
                user = db.Set<SysUser>().Single(m => m.UserName == userName);
            }
            user.AddDate = DateTime.Now;

            using (var db = new DefaultDbContext())
            {
                //通过查询，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Find(user.Id);
                System.Console.WriteLine("更新前：{0}。", oldUser.AddDate);

                DbEntityEntry<SysUser> entryUser = db.Entry(user);
                System.Console.WriteLine("上下文二中user对象的状态：{0}", entryUser.State);//EntityState.Detached

                DbEntityEntry<SysUser> entry = db.Entry(oldUser);
                System.Console.WriteLine("上下文二中oldUser对象的状态1：{0}", entry.State);
                entry.CurrentValues.SetValues(user);
                System.Console.WriteLine("上下文二中oldUser对象的状态2：{0}", entry.State);

                int count = db.SaveChanges();
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新后：{0}。", newUser.AddDate);
            }
        }

        //针对情景二的异常的处理方案封装
        public void Method5()
        {
            const string userName = "张三";

            SysUser user;
            using (var db = new DefaultDbContext())
            {
                user = db.Set<SysUser>().Single(m => m.UserName == userName);
            }
            user.AddDate = DateTime.Now;

            using (var db = new DefaultDbContext())
            {
                //通过查询，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Find(user.Id);
                System.Console.WriteLine("更新前：{0}。", oldUser.AddDate);

                db.Update<SysUser>(user);
                int count = db.SaveChanges();
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.UserName == userName);
                System.Console.WriteLine("更新后：{0}。", newUser.AddDate);
            }
        }

        //场景三：场景二下必须查询出原有对象，才能进行更新操作
        //能不能不查询，就可以更新对象的属性
        //比如修改密码功能，明确只会对密码进行修改
        public void Method6()
        {
            const string userName = "张三";

            long id;
            using (var db = new DefaultDbContext())
            {
                id = db.Set<SysUser>().Single(m => m.UserName == userName).Id;
            }
            SysUser user = new SysUser { Id = id, Password = "NewPassword" + DateTime.Now.Second };

            using (var db = new DefaultDbContext())
            {
                //关闭自动跟踪，方便调试查看
                db.Configuration.AutoDetectChangesEnabled = false;

                db.Database.Log += (log) => { System.Console.WriteLine(log); };

                DbEntityEntry<SysUser> entry = db.Entry(user);
                System.Console.WriteLine("上下文操作前状态：{0}", entry.State);

                //在操作前上下文必须包含对象
                entry.State = EntityState.Unchanged;    //或者db.Set<SysUser>().Attach(user);
                entry.Property("Password").IsModified = true;

                //注意这里的前提是不含版本标识，有版本标识的情况是不支持这种情况的
                db.Configuration.ValidateOnSaveEnabled = false;
                int count = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;

                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.Id == id);
                System.Console.WriteLine("更新后：{0}。", newUser.Password);
            }
        }

        //场景三：当然也会出现类似场景二的InvalidOperationException异常
        //参照场景二进行try catch处理，无效，因为State在被设置 EntityState.Unchanged时，会自动将原值赋值过来
        public void Method7()
        {
            const string userName = "张三";

            long id;
            using (var db = new DefaultDbContext())
            {
                id = db.Set<SysUser>().Single(m => m.UserName == userName).Id;
            }
            SysUser user = new SysUser { Id = id, Password = "NewPassword" + DateTime.Now.Second };

            using (var db = new DefaultDbContext())
            {
                //关闭自动跟踪，方便调试查看
                db.Configuration.AutoDetectChangesEnabled = false;

                db.Database.Log += (log) => { System.Console.WriteLine(log); };

                //通过查询，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Find(user.Id);
                System.Console.WriteLine("更新前：{0}。", oldUser.AddDate);

                try
                {
                    DbEntityEntry<SysUser> entry = db.Entry(user);
                    entry.State = EntityState.Unchanged;
                    entry.Property("Password").IsModified = true;
                }
                catch (InvalidOperationException)
                {
                    DbEntityEntry<SysUser> entry = db.Entry(oldUser);
                    entry.CurrentValues.SetValues(user);
                    entry.State = EntityState.Unchanged;
                    entry.Property("Password").IsModified = true;
                }
                db.Configuration.ValidateOnSaveEnabled = false;
                int count = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;

                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.Id == 1);
                System.Console.WriteLine("更新后：{0}。", newUser.Password);
            }
        }

        //场景三的异常处理，从ObjectContext着手处理
        public void Method8()
        {
            const string userName = "张三";

            long id;
            using (var db = new DefaultDbContext())
            {
                id = db.Set<SysUser>().Single(m => m.UserName == userName).Id;
            }
            SysUser user = new SysUser { Id = id, Password = "NewPassword" + DateTime.Now.Second };

            using (var db = new DefaultDbContext())
            {
                //关闭自动跟踪，方便调试查看
                db.Configuration.AutoDetectChangesEnabled = false;

                //通过查询，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Find(user.Id);
                System.Console.WriteLine("更新前：{0}。", oldUser.Password);

                try
                {
                    DbEntityEntry<SysUser> entry = db.Entry(user);
                    entry.State = EntityState.Unchanged;
                    entry.Property("Password").IsModified = true;
                }
                catch (InvalidOperationException)
                {
                    ObjectContext objectContext = ((IObjectContextAdapter)db).ObjectContext;
                    ObjectStateEntry objectEntry = objectContext.ObjectStateManager.GetObjectStateEntry(oldUser);
                    objectEntry.ApplyCurrentValues(user);
                    objectEntry.ChangeState(EntityState.Unchanged);
                    objectEntry.SetModifiedProperty("Password");
                }

                db.Configuration.ValidateOnSaveEnabled = false;
                int count = db.SaveChanges();
                db.Configuration.ValidateOnSaveEnabled = true;
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.Id == id);
                System.Console.WriteLine("更新后：{0}。", newUser.Password);
            }
        }

        //针对情景三的异常的处理方案封装
        public void Method9()
        {
            SysUser user = new SysUser { Id = 1, Password = "NewPassword" + DateTime.Now.Second };
            using (var db = new DefaultDbContext())
            {
                //先查询一次，让上下文中存在相同主键的对象
                SysUser oldUser = db.Set<SysUser>().Single(m => m.Id == 1);
                System.Console.WriteLine("更新前：{0}。", oldUser.Password);

                db.Update<SysUser>(m => new { m.Password }, user);
                int count = db.SaveChanges(false);
                System.Console.WriteLine("操作结果：{0}", count > 0 ? "更新成功。" : "未更新。");

                SysUser newUser = db.Set<SysUser>().Single(m => m.Id == 1);
                System.Console.WriteLine("更新后：{0}。", newUser.Password);
            }
        }
    }
}
