using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Security;

namespace SomeProject.Core.Model.Account
{
    /// <summary>
    /// 示例实体类——用户
    /// </summary>
    [Description("用户")]
    public class SysUser : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [StringLength(20)]
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// 用户扩展信息
        /// </summary>
        public virtual SysUserExpand UserExpand { get; set; }

        /// <summary>
        /// 用户登录日志
        /// </summary>
        public virtual ICollection<LoginLog> LoginLogs { get; set; }

        /// <summary>
        /// 部门主键
        /// </summary>
        public long? DepartmentId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public virtual SysDepartment Department { get; set; }

        /// <summary>
        /// 用户角色信息集合
        /// </summary>
        public virtual ICollection<SysRole> Roles { get; set; }
    }

    /// <summary>
    /// 实体映射关系
    /// </summary>
    public class SysUserMap : EntityTypeConfiguration<SysUser>
    {
        public SysUserMap()
        {
            #region 功能1：当前对象的表信息设置

            //ToTable("SysUserInfo");
            //HasKey(p => p.Id);
            //HasForeignKey 
            //Property(p => p.UserName).IsRequired().HasMaxLength(50);
            //Ignore(p => p.Description);

            #endregion

            #region 功能2：配置关系，除一对一关系必须配置外，其他都可以直接默认

            //1-1关联
            //在主表中配置时如下，在从表中配置时见SysUserExpand，两者等价
            //从实践角度在从表配置较为合适，标注在依赖中而不是被依赖中符合面向对象的思维
            //HasRequired(p => p.UserExpand).WithRequiredDependent(i => i.User);    //User表关联于UserExpand表
            //HasRequired(p => p.UserExpand).WithOptional(i => i.User);   //User表关联于UserExpand表
            //HasRequired(p => p.UserExpand).WithRequiredPrincipal(i => i.User);  //UserExpand表关联于User表
            //HasOptional(p => p.UserExpand).WithRequired(i => i.User); //UserExpand表关联于User表

            //双向1-*关联
            //在主表中配置时如下，在从表中配置时见LoginLog，两者等价
            //从实践角度在从表配置较为合适，标注在依赖中而不是被依赖中符合面向对象的思维
            //HasMany(p => p.LoginLogs).WithRequired(t => t.User).HasForeignKey(o => o.UserId);

            //单向1-*关联，类似双向1-*关联，区别是哪个是主表
            //HasOptional(p => p.Address).WithMany().HasForeignKey(p => p.AddressId); //可空
            //HasRequired(p => p.Address).WithMany().HasForeignKey(p => p.AddressId); //不可空

            //*-*关联
            //无论是否进行配置，EF都可以识别，方式是新建中间表存储相关两张表的Id信息
            //但特别值得注意的是，这种情况下中间表是不被EF定义获取的，故一种比较好的思路是显式定义一张表做双向1-*关联到相关的两张表
            //HasMany(p => p.Roles).WithMany(t => t.Users).Map(m =>
            //{
            //    m.ToTable("SysUserRole");
            //    m.MapLeftKey("UserId");
            //    m.MapRightKey("RoleId");
            //});

            #endregion

        }
    }
}
