using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace SampleFunctionApp
{
    public class Function1
    {
        private readonly IConfiguration _configuration;

        public Function1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("Function1")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            ILogger log)
        {
            for (int i = 1; i <= 5; i++)
            {
                var key = $"setting{i}";
                log.LogInformation($"{key}: {_configuration[key]}");
            }

            log.LogInformation($"Environment.CurrentDirectory: {Environment.CurrentDirectory}");
            log.LogInformation($"AzureWebJobsScriptRoot: {_configuration["AzureWebJobsScriptRoot"]}");

            return new OkObjectResult($"Check out your logs.");
        }
    }
}
