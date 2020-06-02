using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;

namespace OrderScenario
{
    public static class InsertCart
    {
        [FunctionName("InsertCart")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cart")] 
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cart = JsonConvert.DeserializeObject<Cart>(requestBody);

            var client = new MongoClient(Environment.GetEnvironmentVariable("CosmosDBMongo"));
            var db = client.GetDatabase("store");
            var coll = db.GetCollection<Cart>("cart");
            coll.InsertOne(cart);

            return (ActionResult)new OkResult();
        }
    }
}
