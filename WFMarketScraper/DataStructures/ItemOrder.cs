using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WFMarketScraper.Enums;

namespace WFMarketScraper.DataStructures
{
    public struct ItemOrder
    {
        public string Id;
        public long Platinum;
        public long Quantity;
        public OrderType Type;
        public Platform Platform;
        public DateTime CreationDate;
        public DateTime LastUpdate;
        public User User;

        public ItemOrder(string id, long platinum, long quantity, OrderType type, Platform platform, DateTime creationDate, DateTime lastUpdate, User user)
        {
            Id = id;
            Platinum = platinum;
            Quantity = quantity;
            Type = type;
            Platform = platform;
            CreationDate = creationDate;
            LastUpdate = lastUpdate;
            User = user;
        }

        public ItemOrder(string id, long platinum, long quantity, OrderType type, Platform platform, DateTime creationDate, DateTime lastUpdate, Dictionary<string, JProperty> rawUser)
        {
            Id = id;
            Platinum = platinum;
            Quantity = quantity;
            Type = type;
            Platform = platform;
            CreationDate = creationDate;
            LastUpdate = lastUpdate;
            User = new User(rawUser);
        }

        public ItemOrder(Dictionary<string, object> raw)
        {
            Id = (string)raw["id"];
            Platinum = (long)raw["platinum"];
            Quantity = (long)raw["quantity"];
            Type = (OrderType)Enum.Parse(typeof(OrderType), (string)raw["order_type"]);
            Platform = (Platform)Enum.Parse(typeof(Platform), (string)raw["platform"]);
            CreationDate = (DateTime)raw["creation_date"];
            LastUpdate = (DateTime)raw["last_update"];
            User = new User(((JObject)raw["user"]).Properties().ToDictionary(property => property.Name));
        }

        public override string ToString() =>
            $"Id: {Id},\n" +
            $"Platinum: {Platinum},\n" +
            $"Quantity: {Quantity},\n" +
            $"Type: {Type},\n" +
            $"Platform: {Platform},\n" +
            $"Creation Date: {CreationDate},\n" +
            $"Last Update: {LastUpdate},\n" +
            $"User: {User}";
    }
}
