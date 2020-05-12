using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Data.UnitOfWork
{
    /// <summary>
    /// 业务单元操作接口
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 当前单元操作是否已被提交
        /// </summary>
        bool IsCommitted { get; }

        /// <summary>
        /// 提交当前单元操作的结果
        /// </summary>
        /// <param name="validateOnSaveEnabled">保存时是否自动验证跟踪实体</param>
        /// <returns></returns>
        int Commit(bool validateOnSaveEnabled = true);

        /// <summary>
        /// 提交当前单元操作的结果
        /// </summary>
        /// <param name="validateOnSaveEnabled">保存时是否自动验证跟踪实体</param>
        /// <returns></returns>
        Task<int> CommitAsync(bool validateOnSaveEnabled = true);

        /// <summary>
        /// 把当前单元操作回滚成未提交状态
        /// </summary>
        void Rollback();
    }
}
