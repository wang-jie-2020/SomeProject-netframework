using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Model.Account
{
    /// <summary>
    /// 示例实体类——用户扩展信息，1个User-1个UserExpand
    /// 在实际操作中的知识点：
    /// 此关系在数据库中的表现为主表ID是主键同时也是从表ID的外键
    /// 因已有关联配置故EF不会再为导航属性自动增加id列
    /// </summary>
    [Description("用户扩展信息")]
    public class SysUserExpand : BaseEntity
    {
        /// <summary>
        /// 用户扩展信息1
        /// </summary>
        [StringLength(50)]
        public string ExpandValue1 { get; set; }

        /// <summary>
        /// 用户扩展信息2
        /// </summary>
        [StringLength(50)]
        public string ExpandValue2 { get; set; }

        /// <summary>
        /// 用户扩展信息3
        /// </summary>
        [StringLength(50)]
        public string ExpandValue3 { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        //[Required]    这里出现了一个小插曲，即当User存在，再新增UserExpand时，若此处Required则不能通过EF的编辑验证
        public virtual SysUser User { get; set; }
    }

    /// <summary>
    /// 实体映射关系
    /// </summary>
    public class SysUserExpandMap : EntityTypeConfiguration<SysUserExpand>
    {
        public SysUserExpandMap()
        {
            //1-1关系是必须进行配置的，数据库必须确定哪张表是主哪张表是从，关系到外键的标注
            //方式如下---它等效于在导航属性User上标注[Required]
            HasRequired(p => p.User).WithOptional(i => i.UserExpand);
        }
    }
}
