using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Infrastructure.Common.Helper;
using SomeProject.Infrastructure.Data;

namespace SomeProject.Core.Model
{
    /// <summary>
    /// 项目的基类型
    /// </summary>
    public abstract class BaseEntity : Entity
    {
        //定义项目的通用属性，比如审核状态
    }
}
