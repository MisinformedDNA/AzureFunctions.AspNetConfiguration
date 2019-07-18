using YellowCounter.AzureFunctions.AspNetConfiguration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(SampleFunctionApp.Startup))]
namespace SampleFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var settings = new Dictionary<string, string>
            {
                { "setting5", "memory" }
            };
            builder.UseAspNetConfiguration(c => c.AddInMemoryCollection(settings));

            var configuration = builder.GetConfiguration();
            var value = configuration["setting1"];
        }
    }
}
