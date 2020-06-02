using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderScenario
{
    public static class GetOrder
    {
        [FunctionName("GetOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "order/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "orderdb",
                collectionName: "orders",
                ConnectionStringSetting = "CosmosDBSQL",
                SqlQuery = "select * from c where c.orderId = {id}")] IEnumerable<object> orders,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (orders == null)
            {
                log.LogInformation($"Orders not found");
                return (ActionResult)new OkResult();
            }
            else
            {
                log.LogInformation("Found order");
                foreach(object catalogItem in orders)
                {
                    return new OkObjectResult(catalogItem);
                }
                return (ActionResult)new OkResult();
            }
        }
    }
}
