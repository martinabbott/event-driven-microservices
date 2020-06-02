using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderScenario
{
    public class Order
    {
        public string orderId { get; set; }

        public string customerId { get; set; }

        public List<OrderItem> items { get; set; }

        public float value { get; set; }

        public DateTime createdDate { get; set; }

        public string orderStatus { get; set; }

        public static Order FromCart(Cart cart)
        {
            var orderItems = new List<OrderItem>();

            foreach (CartItem item in cart.items)
            {
                orderItems.Add(new OrderItem{
                    itemId = item.itemId,
                    size = item.size,
                    quantity = item.quantity,
                    unitPrice = item.unitPrice
                });
            }

            var order = new Order{
                orderId = Guid.NewGuid().ToString(),
                customerId = cart.customerId,
                value = cart.value,
                createdDate = DateTime.UtcNow,
                items = orderItems,
                orderStatus = "pending"
            };

            return order;
        }

    }

    public class OrderItem
    {
        public string itemId { get; set; }

        public int size { get; set; }
        
        public int quantity { get; set; }
        
        public float unitPrice { get; set; }
    }
    
    public class OrderEventData
    {
        [JsonProperty("docId")]
        public string DocId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("totalValue")]
        public float TotalValue {get; set; }

        [JsonProperty("orderStatus")]
        public string OrderStatus {get; set; }
    }
}