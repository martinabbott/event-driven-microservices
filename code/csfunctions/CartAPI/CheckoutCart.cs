using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;

namespace OrderScenario
{
    public static class CheckoutCart
    {
        [FunctionName("CheckoutCart")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "cart/checkout/{id}")] HttpRequest req,
            string id,
            [EventGrid(TopicEndpointUri = "CartEventEndpoint", TopicKeySetting = "CartEventKey")] IAsyncCollector<EventGridEvent> outputEvents,
            ILogger log)
        {
            log.LogWarning("CheckoutCart - Updating cart");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var cart = JsonConvert.DeserializeObject<Cart>(requestBody);

            var client = new MongoClient(Environment.GetEnvironmentVariable("CosmosDBMongo"));

            var db = client.GetDatabase("store");
            var coll = db.GetCollection<Cart>("cart");

            var filter = Builders<Cart>.Filter.Eq("cartId", cart.cartId);
            var update = Builders<Cart>.Update.Set("cartStatus", "complete");
            
            coll.UpdateOne(filter, update);
            log.LogWarning("CheckoutCart - Cart updated");

            var evt = new EventGridEvent(
                Guid.NewGuid().ToString(),
                "cart/checkout", 
                cart, 
                "orderSubmitted", 
                DateTime.UtcNow, 
                "1.0");

            await outputEvents.AddAsync(evt);

            log.LogWarning("CheckoutCart - Order event sent");

            return (ActionResult)new OkResult();
        }
    }
}
