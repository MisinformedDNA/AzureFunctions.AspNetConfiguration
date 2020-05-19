using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using YellowCounter.AzureFunctions.AspNetConfiguration;

[assembly: FunctionsStartup(typeof(SampleFunctionAppV3.Startup))]
namespace SampleFunctionAppV3
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.UseAspNetConfiguration();

            var configuration = builder.GetConfiguration();
            var value = configuration["setting1"];
        }
    }
}
