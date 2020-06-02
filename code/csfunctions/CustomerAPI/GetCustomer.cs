using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace OrderScenario
{
    public static class GetCustomer
    {
        [FunctionName("GetCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer/{id}")] 
            HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var str = Environment.GetEnvironmentVariable("SQLDB");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = $"SELECT * FROM customer WHERE CustomerId = '{id}' FOR JSON AUTO";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = cmd.ExecuteScalarAsync().Result.ToString();

                    if (rows == null)
                    {
                        log.LogInformation($"Customers not found");
                        return (ActionResult)new OkResult();
                    }
                    else
                    {
                        log.LogInformation("Found customers");
                        return (ActionResult)new OkObjectResult(rows);
                    }
                }
            }
        }
    }
}
