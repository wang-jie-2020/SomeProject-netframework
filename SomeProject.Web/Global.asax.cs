using SomeProject.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SomeProject.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //设置AutoFac解析器
            System.Web.Mvc.IDependencyResolver autoFacResolver = MvcContainer.Init();    //初始化容器
            DependencyResolver.SetResolver(autoFacResolver);    //将AutoFac解析器设置为系统DI解析器
        }
    }
}
