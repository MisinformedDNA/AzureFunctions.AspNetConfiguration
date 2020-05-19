using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;

namespace YellowCounter.AzureFunctions.AspNetConfiguration
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Use the same configuration providers that are provided in ASP.NET by default.
        /// </summary>
        public static IFunctionsHostBuilder UseAspNetConfiguration(this IFunctionsHostBuilder builder, Action<IConfigurationBuilder> action = null)
        {
#if NETSTANDARD2_0
            var services = builder.Services;
#else
            var services = builder.Services.BuildServiceProvider();
#endif

            var functionsConfig = services.GetService<IConfiguration>();
            var hostingContext = services.GetService<HostBuilderContext>();

            var config = new ConfigurationBuilder()
                .AddConfiguration(functionsConfig)
                .SetBasePath(GetCurrentDirectory());

            var env = hostingContext.HostingEnvironment;
            env.ApplicationName = env.ApplicationName ?? Assembly.GetCallingAssembly().GetName().Name;

            UseAspNetConfiguration(hostingContext, config);
            action?.Invoke(config);

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config.Build()));
            return builder;
        }

        public static IConfiguration GetConfiguration(this IFunctionsHostBuilder builder)
        {
            return builder.Services.GetService<IConfiguration>();
        }

        private static string GetCurrentDirectory()
        {
            return IsLocal() ? Environment.CurrentDirectory : "/home/site/wwwroot";
        }

        private static bool IsLocal()
        {
            return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
        }

        /// <summary>
        /// Uses the same code as found at https://github.com/aspnet/AspNetCore/blob/master/src/DefaultBuilder/src/WebHost.cs.
        /// </summary>
        private static void UseAspNetConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;

            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                {
                    config.AddUserSecrets(appAssembly, optional: true);
                }
            }

            config.AddEnvironmentVariables();
        }

        private static T GetService<T>(this IServiceCollection services) where T : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var service = services.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (service == null) return default;

            return service.ImplementationInstance as T;
        }
    }
}
