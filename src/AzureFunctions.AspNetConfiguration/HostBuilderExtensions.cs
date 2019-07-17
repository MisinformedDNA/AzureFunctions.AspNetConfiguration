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
        /// <param name="builder"></param>
        public static IConfigurationRoot UseAspNetConfiguration(this IFunctionsHostBuilder builder, Action<IConfigurationBuilder> action = null)
        {
            var functionsConfig = builder.Services.GetService<IConfiguration>();
            var hostingContext = builder.Services.GetService<HostBuilderContext>();

            var config = new ConfigurationBuilder()
                .AddConfiguration(functionsConfig)
                .SetBasePath(GetCurrentDirectory());

            IHostingEnvironment env = hostingContext.HostingEnvironment;
            env.ApplicationName = env.ApplicationName ?? Assembly.GetCallingAssembly().GetName().Name;

            UseAspNetConfiguration(hostingContext, config);

            action?.Invoke(config);



            IConfigurationRoot builtConfig = config.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), builtConfig));
            return builtConfig;
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
