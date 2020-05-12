using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Helper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SomeProject.Core.Model.Account
{
    /// <summary>
    /// 示例实体类——登录日志，1个User-*个LoginLog
    /// 在实际操作中的知识点：
    /// 此关系在数据库中的表现为从表中存在主表ID的外键
    /// 故若外键列缺省，EF也会默认添加
    /// </summary>
    [Description("登录日志")]
    public class LoginLog : BaseEntity
    {
        /// <summary>
        /// 登录IP地址
        /// </summary>
        [Required]
        [StringLength(15)]
        public string IpAddress { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual SysUser User { get; set; }
    }

    /// <summary>
    /// 实体映射关系
    /// </summary>
    public class LoginLogMap : EntityTypeConfiguration<LoginLog>
    {
        public LoginLogMap()
        {
            //双向的1-*关系，若不加标注EF可以自动识别
            //方式如下---等效于外键属性[ForeignKey(nameof(User))]或导航属性[ForeignKey("UserId")]
            //HasRequired(t => t.User).WithMany(p => p.LoginLogs).HasForeignKey(o => o.UserId);
        }
    }
}
