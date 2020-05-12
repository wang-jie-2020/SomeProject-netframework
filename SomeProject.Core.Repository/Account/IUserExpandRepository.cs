using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Infrastructure.Data.Repository;

namespace SomeProject.Core.Repository.Account
{
    public interface IUserExpandRepository : IRepository<SysUserExpand>
    {
    }
}
