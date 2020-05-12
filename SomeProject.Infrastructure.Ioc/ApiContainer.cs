using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.WebApi;

namespace SomeProject.Infrastructure.Ioc
{
    /// <summary>
    /// .NET Framework WebApi容器
    /// </summary>
    public static class ApiContainer
    {
        public static IContainer Instance { get; private set; }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="func">需额外进行的一些类型注册</param>
        /// <returns></returns>
        public static System.Web.Http.Dependencies.IDependencyResolver Init(Func<ContainerBuilder, ContainerBuilder> func = null)
        {
            //新建容器构建器，用于注册组件和服务
            var builder = new ContainerBuilder();

            //注册默认组件
            WebApiBuild(builder);

            //注册非默认组件
            func?.Invoke(builder);

            //利用构建器创建容器
            Instance = builder.Build();

            //返回针对WebApi的AutoFac解析器
            return new AutofacWebApiDependencyResolver(Instance);
        }

        /// <summary>
        /// 注册默认组件
        /// </summary>
        /// <param name="builder"></param>
        static void WebApiBuild(ContainerBuilder builder)
        {
            var assemblies = System.Web.Compilation.BuildManager
                            .GetReferencedAssemblies()
                            .Cast<Assembly>()
                            .Where(x => x.FullName.Contains("SomeProject"))
                            .ToArray();

            //注册 Repository && Service
            builder.RegisterAssemblyTypes(assemblies)
                            .Where(cc => cc.Name.EndsWith("Repository") || cc.Name.EndsWith("Service"))
                            .PublicOnly()
                            .Where(cc => cc.IsClass)
                            .AsImplementedInterfaces()
                            .PropertiesAutowired(); //属性注入

            //或通过泛型注册进行，如builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>));

            //注册 ApiController
            Assembly assembly = assemblies.FirstOrDefault(x => x.FullName.Contains("Api"));
            builder.RegisterApiControllers(assembly).PropertiesAutowired(); //属性注入

            //或通过RegisterApiControllers的实现进行个性化注册
            //Assembly[] controllerAssemblies = assemblies.Where(x => x.FullName.Contains(".NetFrameworkApi")).ToArray();
            //builder.RegisterAssemblyTypes(controllerAssemblies)
            //    .Where(cc => cc.Name.EndsWith("Controller"))
            //    .AsSelf();
        }
    }
}
