using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace OrderScenario
{
    [BsonIgnoreExtraElements]
    public class Cart
    {
        [BsonElement("cartId")]
        public string cartId { get; set; }

        [BsonElement("customerId")]
        public string customerId { get; set; }

        [BsonElement("items")]
        public List<CartItem> items { get; set; }

        [BsonElement("value")]
        public float value { get; set; }

        [BsonElement("createdDate")]
        public DateTime createdDate { get; set; }

        [BsonElement("cartStatus")]
        public string cartStatus { get; set; }
    }

    public class CartItem
    {
        [BsonElement("itemId")]
        public string itemId { get; set; }

        [BsonElement("size")]
        public int size { get; set; }
        
        [BsonElement("quantity")]
        public int quantity { get; set; }
        
        [BsonElement("unitPrice")]
        public float unitPrice { get; set; }
    }
}