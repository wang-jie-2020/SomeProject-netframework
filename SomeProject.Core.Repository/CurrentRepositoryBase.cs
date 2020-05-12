using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Data;
using SomeProject.Infrastructure.Data.Repository;
using SomeProject.Infrastructure.Data.UnitOfWork;

namespace SomeProject.Core.Repository
{
    public class CurrentRepositoryBase<TEntity> : RepositoryBase<TEntity> where TEntity : Entity
    {
        //泛型类的静态字段只在形参一致时才共享，不同形参是不同的静态域
        //private static UnitOfWorkContextBase _UnitOfWorkContext;

        protected override UnitOfWorkContextBase UnitOfWorkContext
        {
            get
            {
                //if (_UnitOfWorkContext == null)
                //{
                //    _UnitOfWorkContext = new CurrentUnitOfWorkContext();
                //}

                //return _UnitOfWorkContext;

                //设计思路是在不同的仓储中共享状态，即必然是同一个UnitOfWork，则也就是同一个DbContext
                return Singleton<CurrentUnitOfWorkContext>.Instance;
            }
        }

    }
}
