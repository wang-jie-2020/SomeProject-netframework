using SomeProject.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SomeProject.Infrastructure.Common.Extensions;
using SomeProject.Application.ViewModel.Account;
using SomeProject.Application.Domain.Account;

namespace SomeProject.WebApi.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        public IAccountAppService AccountAppService { get; set; }

        [HttpPost]
        [Route("login")]
        public OperationResult Login(LoginViewModel model)
        {
            try
            {
                OperationResult result = AccountAppService.Login(model);
                string msg = result.Message ?? result.ResultType.ToDescription();
                return result;
            }
            catch (Exception e)
            {
                return new OperationResult(OperationResultType.Error, e.Message);
            }
        }
    }
}
