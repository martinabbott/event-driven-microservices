using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;

namespace OrderScenario
{
    public static class OrderPayment
    {
        [FunctionName("OrderPayment")]
        public static async void Run([CosmosDBTrigger(
            databaseName: "orderdb",
            collectionName: "orders",
            ConnectionStringSetting = "CosmosDBSQL",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, 
            [EventGrid(TopicEndpointUri = "PaymentEventEndpoint", TopicKeySetting = "PaymentEventKey")] IAsyncCollector<EventGridEvent> outputEvents,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogWarning("OrderPayment - Processing Order change feed");

                var order = JsonConvert.DeserializeObject<Order>(input[0].ToString());

                var evtData = new OrderEventData{
                    OrderId = order.orderId,
                    TotalValue = order.value,
                    DocId = input[0].Id
                };
                
                if (order.orderStatus == "pending")
                {
                    log.LogWarning("OrderPayment - Processing pending Order");
                    
                    var evt = new EventGridEvent(
                        Guid.NewGuid().ToString(),
                        "order/status/pending", 
                        evtData, 
                        "paymentRequest", 
                        DateTime.UtcNow, 
                        "1.0");

                    await outputEvents.AddAsync(evt);

                    log.LogWarning("OrderPayment - Sending event to Payment");
                }
            }
        }
    }

}
