using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Newtonsoft.Json;

namespace OrderScenario
{
    public static class OrderProcess
    {
        [FunctionName("OrderProcess")]
       public static async void Run(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogWarning("OrderProcess - Processing payment response event");

            var orderEvent = JsonConvert.DeserializeObject<OrderEventData>(eventGridEvent.Data.ToString());

            if (orderEvent.OrderStatus == "accepted")
            {
                log.LogWarning("OrderProcess - Updating Order to accepted");
                using (var docClient = new DocumentClient(new Uri(Environment.GetEnvironmentVariable("CosmosDBSQLEndpoint")),Environment.GetEnvironmentVariable("CosmosDBSQLKey")))
                {
                    var feedOptions = new FeedOptions { EnableCrossPartitionQuery = true };
                    var order = docClient.CreateDocumentQuery<Document>(
                        UriFactory.CreateDocumentCollectionUri("orderdb", "orders"), feedOptions)
                        .Where(o => o.Id == orderEvent.DocId)
                        .AsEnumerable();
                    
                    if (order != null)
                    {
                        if (order.Count() > 0)
                        {
                            var myorder = order.First();

                            if (myorder.GetPropertyValue<string>("orderStatus") == "pending")
                            {
                                myorder.SetPropertyValue("orderStatus",  "accepted");
                                
                                var uri = myorder.SelfLink;

                                if (uri != null)
                                {
                                    var response = await docClient.ReplaceDocumentAsync(uri, myorder);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
