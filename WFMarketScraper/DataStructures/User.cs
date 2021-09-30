using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WFMarketScraper.Enums;

namespace WFMarketScraper.DataStructures
{
    public struct User
    {
        public string Id;
        public string IngameName;
        public UserStatus Status;
        public string Region;
        public long Reputation;
        public string Avatar;
        public DateTime LastSeen;

        public User(string id, string ingameName, UserStatus status, string region, long reputation, string avatar, DateTime lastSeen)
        {
            Id = id;
            IngameName = ingameName;
            Status = status;
            Region = region;
            Reputation = reputation;
            Avatar = avatar;
            LastSeen = lastSeen;
        }

        public User(Dictionary<string, JProperty> raw)
        {
            Id = (string)raw["id"];
            IngameName = (string)raw["ingame_name"];
            Status = (UserStatus)Enum.Parse(typeof(UserStatus), (string)raw["status"]);
            Region = (string)raw["region"];
            Reputation = (long)raw["reputation"];
            Avatar = (string)raw["avatar"];
            LastSeen = (DateTime)raw["last_seen"];
        }

        public override string ToString() =>
            $"[Id: {Id}, " +
            $"IGN: {IngameName}, " +
            $"Status: {Status}, " +
            $"Regin: {Region}, " +
            $"Reputation: {Reputation}, " +
            $"Avatar: {Avatar}, " +
            $"LastSeen: {LastSeen}]";
    }
}
