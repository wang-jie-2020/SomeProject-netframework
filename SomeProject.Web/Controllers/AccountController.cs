using SomeProject.Application.Domain.Account;
using SomeProject.Application.ViewModel.Account;
using SomeProject.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SomeProject.Infrastructure.Common.Extensions;
using SomeProject.Infrastructure.Common.Logging;

namespace SomeProject.Web.Controllers
{
    public class AccountController : Controller
    {
        public IAccountAppService AccountAppService { get; set; }

        [HttpGet]
        public ActionResult Login()
        {
            string returnUrl = Request.Params["returnUrl"];
            returnUrl = returnUrl ?? Url.Action("Index", "Home", new { area = "" });
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                //Logger logger = Logger.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                //logger.Info("aaa");
                //logger.Debug("bbb");
                //logger.Error("ccc");

                OperationResult result = AccountAppService.Login(model);
                string msg = result.Message ?? result.ResultType.ToDescription();
                if (result.ResultType == OperationResultType.Success)
                {
                    return Redirect(model.ReturnUrl);
                }
                ModelState.AddModelError("", msg);
                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            string returnUrl = Request.Params["returnUrl"];
            returnUrl = returnUrl ?? Url.Action("Index", "Home", new { area = "" });
            if (User.Identity.IsAuthenticated)
            {
                AccountAppService.Logout();
            }
            return Redirect(returnUrl);
        }
    }
}