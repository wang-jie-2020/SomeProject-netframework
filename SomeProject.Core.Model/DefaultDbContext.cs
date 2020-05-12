using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;

namespace SomeProject.Core.Model
{
    /// <summary>
    /// 数据访问上下文
    /// </summary>
    public class DefaultDbContext : DbContext
    {
        #region 构造函数

        /// <summary>
        ///     初始化一个 使用连接名称为“default”的数据访问上下文类 的新实例
        /// </summary>
        public DefaultDbContext() : base("default")
        {
            //初始化器，在正式的运行良好的项目中应设置初始化器为null防止误操作
            //Database.SetInitializer<DefaultDbContext>(null);

            //调试时直接最新版本
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefaultDbContext, Migrations.Configuration>());

            DbInterception.Add(new DefaultDbCommandInterceptor());

            //Database.Log = (log) => { Console.WriteLine(log); };
        }

        #endregion

        #region 属性

        public DbSet<SysUser> Users { get; set; }

        public DbSet<SysUserExpand> UserExpands { get; set; }

        public DbSet<SysRole> Roles { get; set; }

        public DbSet<LoginLog> LoginLogs { get; set; }

        public DbSet<SysDepartment> Departments { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //移除复数表名
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //移除一对多的级联删除约定，想要级联删除可以在EntityTypeConfiguration<T>的实现类中进行控制
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //移除多对多的级联删除约定，想要级联删除可以在EntityTypeConfiguration<T>的实现类中进行控制
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //关系配置
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
