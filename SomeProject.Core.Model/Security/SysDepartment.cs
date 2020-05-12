using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Model.Security
{
    /// <summary>
    /// 示例实体类——部门，1个部门-*个User
    /// 在实际操作中的知识点：（单向1-*）
    /// 此关系在数据库中的表现为主表ID是主键同时也是从表ID的外键（和LoginLog一致，但注意主表在指向不一致）
    /// 它的各项设置均可参照LoginLog，关系设置很明显位于User上
    /// </summary>
    [Description("部门")]
    public class SysDepartment : BaseEntity
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }
    }
}
