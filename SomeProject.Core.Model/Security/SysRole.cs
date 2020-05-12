using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;

namespace SomeProject.Core.Model.Security
{
    /// <summary>
    /// 示例实体类——角色信息，*用户-*角色
    /// </summary>
    [Description("角色信息")]
    public class SysRole : BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public RoleType RoleType { get; set; }

        /// <summary>
        /// 用户集合
        /// </summary>
        public virtual ICollection<SysUser> Users { get; set; }
    }

    /// <summary>
    /// 实体映射关系
    /// </summary>
    public class SysRoleMap : EntityTypeConfiguration<SysRole>
    {
        public SysRoleMap()
        {

        }
    }
}
