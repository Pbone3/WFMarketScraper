using System;
using System.Net;
using System.Threading.Tasks;
using WFMarketScraper.Enums;

namespace WFMarketScraper.DataStructures
{
    public struct WFWebRequest
    {
        public string ApiVersion;
        public string[] Path;
        public WebRequestType Type;

        private bool Sent;
        private WebResponse Response;

        public WFWebRequest(WebRequestType type, string apiVersion = "v1", params string[] path)
        {
            ApiVersion = apiVersion;
            Path = path;
            Type = type;

            Sent = false;
            Response = null;
        }

        public async Task<WebResponse> SendAndGetReponseAsync()
        {
            HttpWebRequest request = WebRequest.CreateHttp(GetURI());
            request.Method = Type.ToString();

            Sent = true;
            return await request.GetResponseAsync();
        }

        public void Send()
        {
            HttpWebRequest request = WebRequest.CreateHttp(GetURI());
            request.Method = Type.ToString();

            Response = request.GetResponse();
            Sent = true;
        }

        public WebResponse GetResponse()
        {
            if (!Sent)
                throw new Exception($"Request to {GetURI()} has not yet been sent.");

            return Response;
        }

        public string GetURI() => $"http://api.warframe.market/v1/{string.Join('/', Path)}";
    }
}
