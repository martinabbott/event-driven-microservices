using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;

namespace OrderScenario
{
    public static class OrderSubmitted
    {
        [FunctionName("OrderSubmitted")]
        public static void Run(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [CosmosDB(
                databaseName: "orderdb",
                collectionName: "orders",
                ConnectionStringSetting = "CosmosDBSQL")]out dynamic document,
            ILogger log)
        {
            log.LogWarning("OrderSumbitted - Processing checkout event and using Cosmos DB output binding");

            document = Order.FromCart(JsonConvert.DeserializeObject<Cart>(eventGridEvent.Data.ToString()));
        }
    }
}
