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
    public static class GetOrders
    {
        [FunctionName("GetOrders")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "order")] HttpRequest req,
            [CosmosDB(
                databaseName: "orderdb",
                collectionName: "orders",
                ConnectionStringSetting = "CosmosDBSQL",
                SqlQuery = "select * from c")] IEnumerable<object> orders,
            ILogger log)
        {
            log.LogWarning("GetOrders - Using Cosmos DB input binding");
            if (orders == null)
            {
                log.LogInformation($"Orders not found");
                return (ActionResult)new OkResult();
            }
            else
            {
                return (ActionResult)new OkObjectResult(orders);
            }
        }
    }
}
