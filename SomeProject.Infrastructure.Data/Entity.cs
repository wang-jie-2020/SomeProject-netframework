using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Data
{
    /// <summary>
    /// 可持久到数据库的领域模型的基类。
    /// </summary>
    [Serializable]
    public abstract class Entity : DisposableObject
    {
        /// <summary>
        /// 主键
        /// 思考：是否应当定义为泛型的主键？
        /// 过程记录：1.实践下来成本很高；2.主键在一个项目中可能经常不一样？不可能吧
        /// </summary>
        public long Id { get; set; }
        //public Guid Id { get; set; } = CombHelper.NewComb();

        /// <summary>
        /// 是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        //[NotMapped]   是否要映射数据库字段？尝试了一下不加以映射对出现错误，因为model和数据库不一致
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 添加时间
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime AddDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 版本控制标识，用于处理并发
        /// </summary>
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
