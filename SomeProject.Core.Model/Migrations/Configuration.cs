using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;

namespace SomeProject.Core.Model.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SomeProject.Core.Model.DefaultDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            //是否允许自动迁移时的数据丢失，慎用
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SomeProject.Core.Model.DefaultDbContext context)
        {
            base.Seed(context);
            return;

            List<SysRole> roles = new List<SysRole>
            {
                new SysRole{ Name = "系统管理", Description = "系统管理角色，拥有整个系统的管理权限。", RoleType = RoleType.Admin},
                new SysRole{ Name = "蓝钻", Description = "蓝钻会员角色", RoleType = RoleType.Guest},
                new SysRole{ Name = "红钻", Description = "红钻会员角色", RoleType = RoleType.Guest},
                new SysRole{ Name = "黄钻", Description = "黄钻会员角色", RoleType = RoleType.Guest},
                new SysRole{ Name = "绿钻", Description = "绿钻会员角色", RoleType = RoleType.Guest}
            };
            DbSet<SysRole> roleSet = context.Set<SysRole>();
            roleSet.AddOrUpdate(m => new { m.Name }, roles.ToArray());
            context.SaveChanges();

            List<SysUser> users = new List<SysUser>
            {
                new SysUser { UserName = "admin", Password = "123456", Email = "admin@cn.net", NickName = "管理员" },
                new SysUser { UserName = "zhangsan", Password = "123456", Email = "zhangsan@cn.net", NickName = "张三" }
            };

            for (int i = 0; i < 100; i++)
            {
                Random rnd = new Random((int)DateTime.Now.Ticks + i);
                SysUser user = new SysUser
                {
                    UserName = "userName" + i,
                    Password = "123456",
                    Email = "userName" + i + "@cn.net",
                    NickName = "用户" + i
                };
                var roleArray = roleSet.ToArray();
                if (user.Roles == null)
                    user.Roles = new List<SysRole>();
                user.Roles.Add(roleArray[rnd.Next(0, roleArray.Length)]);
                if (rnd.NextDouble() > 0.5)
                {
                    user.Roles.Add(roleArray[rnd.Next(1, roleArray.Length)]);
                }
                users.Add(user);
            }
            DbSet<SysUser> userSet = context.Set<SysUser>();
            userSet.AddOrUpdate(m => new { m.UserName }, users.ToArray());
        }
    }
}
