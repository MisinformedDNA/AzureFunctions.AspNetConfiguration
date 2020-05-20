# AzureFunctions.AspNetConfiguration
Adds ASP.NET Core Configuration capabilities to Azure Functions

Azure Functions typically relies on local.settings.json and Azure environment variables for appsettings. 
If you are here, you want an experience similar to ASP.NET.

[![Build Status](https://dev.azure.com/yellowcounter/GitHub/_apis/build/status/AzureFunctions.AspNetConfiguration?branchName=master)](https://dev.azure.com/yellowcounter/GitHub/_build/latest?definitionId=26&branchName=master)

[![Nuget](https://img.shields.io/nuget/v/AzureFunctions.AspNetConfiguration)](https://www.nuget.org/packages/AzureFunctions.AspNetConfiguration/)

## Configure

To configure this plugin, add a NuGet reference to [AzureFunctions.AspNetConfiguration](https://www.nuget.org/packages/AzureFunctions.AspNetConfiguration/)
and then add `builder.UseAspNetConfiguration()` to Startup.cs, like:

```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using YellowCounter.AzureFunctions.AspNetConfiguration;

[assembly: FunctionsStartup(typeof(SampleFunctionApp.Startup))]
namespace SampleFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.UseAspNetConfiguration();
        }
    }
}
```

## Complex objects
Unlike local.settings.json, appsettings.json supports complex/sectioned settings such as

```json
{
  "section": {
    "hello": "world"
  },
}
```

They are retrieved via standard means, as one would expect.

## Configuration Hierarchy
The configuration added ASP.NET configuration *on top* of the default Azure Functions configuration settings. So we get this hierarchy:

1. local.settings.json (by default)
2. Environment variables (by default)
3. appsettings.json
4. appsettings.{EnvironmentName}.json
  - EnvironmentName is pulled from the value if `AZURE_FUNCTIONS_ENVIRONMENT`
    - In Visual Studio, this value defaults to 'Development'
    - In Azure, this value defaults to 'Production'
    - I'm [following up](https://github.com/MicrosoftDocs/azure-docs/issues/36045#issuecomment-517383100) to determine how to best override this value locally.
    - You can override using `ASPNETCORE_ENVIRONMENT`, but it is [not recommended](https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings#azure_functions_environment)
5. User Secrets, if `AZURE_FUNCTIONS_ENVIRONMENT` is set to 'Development'
6. Environment variables (again, to give it the highest priority)

## Extend
If you would like to use other ASP.NET config providers, you can add others to the end (and thus give them  highest priority), for example:

```csharp
var settings = new Dictionary<string, string>
{
    { "setting5", "memory" }
};
builder.UseAspNetConfiguration(c => c.AddInMemoryCollection(settings).AddEnvironmentVariables());
```

## Retrieve configuration
In order to retrieve the configuration settings themselves, you can use:

```csharp
var configuration = builder.GetConfiguration();
var value = configuration["setting1"];
```
