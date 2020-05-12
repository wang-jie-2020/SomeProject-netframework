using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace SomeProject.Infrastructure.Ioc
{
    /// <summary>
    /// 控制台程序容器
    /// </summary>
    public static class Container
    {
        public static IContainer Instance { get; private set; }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="func">需额外进行的一些类型注册</param>
        /// <returns></returns>
        public static void Init(Func<ContainerBuilder, ContainerBuilder> func = null)
        {
            //新建容器构建器，用于注册组件和服务
            var builder = new ContainerBuilder();

            //注册默认组件
            DefaultBuild(builder);

            //注册非默认组件
            func?.Invoke(builder);

            //利用构建器创建容器
            Instance = builder.Build();
        }

        /// <summary>
        /// 注册默认组件
        /// </summary>
        /// <param name="builder"></param>
        public static void DefaultBuild(ContainerBuilder builder)
        {
            Func<Assembly[]> getAssembly = () =>
            {
                List<Assembly> ass = new List<Assembly>();

                var files = System.IO.Directory.GetFiles(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "SomeProject.*.dll");
                foreach (var f in files)
                {
                    ass.Add(Assembly.LoadFrom(f));
                }

                Assembly assembly = Assembly.GetEntryAssembly();
                ass.Add(assembly);

                return ass.ToArray();
            };

            Assembly[] assemblies = getAssembly();

            //注册 Repository && Service
            builder.RegisterAssemblyTypes(assemblies)
                .Where(cc => cc.Name.EndsWith("Repository") || cc.Name.EndsWith("Service"))
                .PublicOnly()
                .Where(cc => cc.IsClass)
                .AsImplementedInterfaces()
                .PropertiesAutowired(); //属性注入
        }
    }
}
