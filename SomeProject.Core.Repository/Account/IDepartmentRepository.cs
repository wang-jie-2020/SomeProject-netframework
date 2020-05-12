using SomeProject.Infrastructure.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Core.Model.Account;
using SomeProject.Core.Model.Security;

namespace SomeProject.Core.Repository.Account
{
    public interface IDepartmentRepository : IRepository<SysDepartment>
    {
    }
}
