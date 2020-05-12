using SomeProject.Core.Model.Account;
using SomeProject.Infrastructure.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Core.Repository.Account
{
    public interface ILoginLogRepository : IRepository<LoginLog>
    {
    }
}
