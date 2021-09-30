using WFMarketScraper.Enums;

namespace WFMarketScraper.DataStructures
{
    public static class WFWebRequestPresets
    {
        public static WFWebRequest GetItemOrders(string item) => new WFWebRequest(WebRequestType.GET, "v1", "items", item, "orders");
    }
}
