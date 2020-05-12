using SomeProject.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SomeProject.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //设置AutoFac解析器
            System.Web.Http.Dependencies.IDependencyResolver autoFacResolver = ApiContainer.Init();  //初始化容器
            HttpConfiguration config = GlobalConfiguration.Configuration;   //获取HttpConfiguration
            config.DependencyResolver = autoFacResolver;    //将AutoFac解析器设置为系统DI解析器
        }
    }
}
