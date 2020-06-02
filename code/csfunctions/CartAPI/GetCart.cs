using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using MongoDB.Driver;

namespace OrderScenario
{
    public static class GetCart
    {
        [FunctionName("GetCart")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cart/{id}")] 
            HttpRequest req, 
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var client = new MongoClient(Environment.GetEnvironmentVariable("CosmosDBMongo"));

            var db = client.GetDatabase("store");
            var coll = db.GetCollection<Cart>("cart");
            try
            {
                var item = coll.Find<Cart>(cart => cart.cartId == id).FirstOrDefault();
                return (ActionResult)new OkObjectResult(item);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return (ActionResult)new BadRequestObjectResult(e);
            }
        }
    }
}
