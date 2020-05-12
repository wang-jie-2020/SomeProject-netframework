using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Infrastructure.Data.Repository;
using SomeProject.Infrastructure.Data.UnitOfWork;

namespace SomeProject.Core.Repository.Account.impl
{
    public class UserRepository : CurrentRepositoryBase<SysUser>, IUserRepository
    {
    }
}
