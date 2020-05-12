using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Data.UnitOfWork;

namespace SomeProject.Core.Repository
{
    /// <summary>
    /// 上下文类
    /// 思考的一个问题：这里需要进行注入？参考的资料上是注入的，但个人认为过度设计了
    /// </summary>
    public class CurrentUnitOfWorkContext : UnitOfWorkContextBase
    {
        private DbContext _Context;

        /// <summary>
        /// 当前项目的数据访问上下文对象
        /// </summary>
        protected override DbContext Context
        {
            get
            {
                if (_Context == null)
                {
                    _Context = new DefaultDbContext();
                }

                return _Context;
            }
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    _Context = null;
        //}
    }
}
