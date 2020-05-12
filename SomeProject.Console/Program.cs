using SomeProject.Application.Domain.Account;
using SomeProject.Application.ViewModel.Account;
using SomeProject.Infrastructure.Common;
using SomeProject.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeProject.Infrastructure.Common.Extensions;

namespace SomeProject.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //框架中的登录测试
            //new AutoFacTest().Login();

            //EF测试
            //EfQueryTest.RunTest();
            //EfUpdateTest.RunTest();
            //EfOneToOneTest.RunTest();
            //EfOneToManyTest.RunTest();
            EfManyToManyTest.RunTest();
        }
    }
}
