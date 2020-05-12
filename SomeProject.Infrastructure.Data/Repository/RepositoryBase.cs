using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Common.Exceptions;
using SomeProject.Infrastructure.Common.Helper;
using SomeProject.Infrastructure.Data.UnitOfWork;

namespace SomeProject.Infrastructure.Data.Repository
{
    /// <summary>
    /// EntityFramework仓储操作基类
    /// </summary>
    /// <typeparam name="TEntity">动态实体类型</typeparam>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected abstract UnitOfWorkContextBase UnitOfWorkContext { get; }

        /// <summary>
        /// 当前实体的查询数据集
        /// </summary>
        public virtual IQueryable<TEntity> Entities
        {
            get
            {
                //从实际来看，这里AsNoTracking()提高性能但主要注意就可以；
                //不AsNoTracking的话写业务逻辑比较方便
                return UnitOfWorkContext.Set<TEntity>().Where(t => !t.IsDeleted).AsNoTracking();
            }
        }

        #region 同步方法

        /// <summary>
        /// 查找指定主键的实体记录
        /// </summary>
        /// <param name="key"> 指定主键 </param>
        /// <returns> 符合编号的记录，不存在返回null </returns>
        public virtual TEntity Query(object key)
        {
            PublicHelper.CheckArgument(key, "key");
            return UnitOfWorkContext.Set<TEntity>().Find(key);
        }

        /// <summary>
        /// 插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Insert(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterNew(entity);
            return isSave ? Save() : 0;
        }

        /// <summary>
        /// 批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Insert(IEnumerable<TEntity> entities, bool isSave = true)
        {
            PublicHelper.CheckArgument(entities, "entities");
            UnitOfWorkContext.RegisterNew(entities);
            return isSave ? Save() : 0;
        }

        /// <summary>
        ///  删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Delete(object id, bool isSave = true)
        {
            PublicHelper.CheckArgument(id, "id");
            TEntity entity = UnitOfWorkContext.Set<TEntity>().Find(id);
            return entity != null ? Delete(entity, isSave) : 0;
        }

        /// <summary>
        ///  删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Delete(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterDeleted(entity);
            return isSave ? Save() : 0;
        }

        /// <summary>
        ///  删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Delete(IEnumerable<TEntity> entities, bool isSave = true)
        {
            PublicHelper.CheckArgument(entities, "entities");
            UnitOfWorkContext.RegisterDeleted(entities);
            return isSave ? Save() : 0;
        }

        /// <summary>
        /// 更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Update(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterModified(entity);
            return isSave ? Save() : 0;
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <returns></returns>
        public virtual int Save()
        {
            return UnitOfWorkContext.Commit();
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// 查找指定主键的实体记录
        /// </summary>
        /// <param name="key"> 指定主键 </param>
        /// <returns> 符合编号的记录，不存在返回null </returns>
        public virtual async Task<TEntity> QueryAsync(object key)
        {
            PublicHelper.CheckArgument(key, "key");
            return await UnitOfWorkContext.Set<TEntity>().FindAsync(key);
        }

        /// <summary>
        /// 插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> InsertAsync(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterNew(entity);
            return isSave ? await SaveAsync() : 0;
        }

        /// <summary>
        /// 批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> InsertAsync(IEnumerable<TEntity> entities, bool isSave = true)
        {
            PublicHelper.CheckArgument(entities, "entities");
            UnitOfWorkContext.RegisterNew(entities);
            return isSave ? await SaveAsync() : 0;
        }

        /// <summary>
        ///  删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> DeleteAsync(object id, bool isSave = true)
        {
            PublicHelper.CheckArgument(id, "id");
            TEntity entity = UnitOfWorkContext.Set<TEntity>().Find(id);
            return entity != null ? await SaveAsync() : 0;
        }

        /// <summary>
        /// 删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> DeleteAsync(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterDeleted(entity);
            return isSave ? await SaveAsync() : 0;
        }

        /// <summary>
        /// 删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities, bool isSave = true)
        {
            PublicHelper.CheckArgument(entities, "entities");
            UnitOfWorkContext.RegisterDeleted(entities);
            return isSave ? await SaveAsync() : 0;
        }

        /// <summary>
        /// 更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual async Task<int> UpdateAsync(TEntity entity, bool isSave = true)
        {
            PublicHelper.CheckArgument(entity, "entity");
            UnitOfWorkContext.RegisterModified(entity);
            return isSave ? await SaveAsync() : 0;
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> SaveAsync()
        {
            return await UnitOfWorkContext.CommitAsync();
        }

        #endregion
    }
}
