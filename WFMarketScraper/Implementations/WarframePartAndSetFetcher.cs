using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WFMarketScraper.DataStructures;

namespace WFMarketScraper.Backend
{
    public static class WarframePartAndSetFetcher
    {
        public static Dictionary<string, Dictionary<string, ItemOrder[]>> RecieveOrders()
        {
            Dictionary<string, Dictionary<string, ItemOrder[]>> output = new Dictionary<string, Dictionary<string, ItemOrder[]>>();

            foreach (string warframe in Utils.PrimeWarframeNames)
            {
                string[] parts = Utils.GetPartNames(warframe);

                List<Task<(string item, ItemOrder[] orders)>> activeRequests = new List<Task<(string item, ItemOrder[] orders)>>();

                foreach (string part in parts)
                {
                    if (activeRequests.Count >= 3)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        while (!activeRequests.Any((Task<(string part, ItemOrder[] orders)> task) => task.IsCompleted))
                        {
                            ;
                        }

                        foreach (Task<(string item, ItemOrder[] orders)> req in activeRequests)
                        {
                            if (req.IsCompleted)
                                output[warframe][req.Result.item] = req.Result.orders;
                        }

                        activeRequests.RemoveAll((Task<(string item, ItemOrder[] orders)> task) => task.IsCompleted);

                        stopwatch.Stop();
                        int ms = (int)stopwatch.ElapsedMilliseconds;

                        if (ms < 1000)
                            Thread.Sleep(1000 - ms);
                    }

                    Task<(string item, ItemOrder[] orders)> task = Task.Run(async () => {
                        WebResponse response = await WFWebRequestPresets.GetItemOrders(part).SendAndGetReponseAsync();

                        string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        JObject parsedResponse = JObject.Parse(json);
                        Dictionary<string, object>[] orders = parsedResponse.SelectToken("payload").SelectToken("orders").ToObject<Dictionary<string, object>[]>();

                        return (part, orders.Select((Dictionary<string, object> raw) => new ItemOrder(raw)).ToArray());
                    });

                    activeRequests.Add(task);
                }
            }

            return output;
        }

        public static IEnumerable<(string warframe, Dictionary<string, ItemOrder[]> parts)> RecieveAndEnumerateOrders()
        {
            List<string> itemsToQuery = new List<string>();

            foreach (string warframe in Utils.PrimeWarframeNames)
            {
                string[] parts = Utils.GetPartNames(warframe);

                foreach (string part in parts)
                {
                    itemsToQuery.Add(part);
                }
            }

            List<Task<(string item, ItemOrder[] orders)>> activeRequests = new List<Task<(string item, ItemOrder[] orders)>>();
            Dictionary<string, ItemOrder[]> output = new Dictionary<string, ItemOrder[]>();
            Dictionary<string, int> warframesToPartsCompleted = new Dictionary<string, int>();

            foreach (string part in itemsToQuery)
            {
                if (activeRequests.Count >= 3)
                {
                    WaitForRequestsToFinish(activeRequests, output, warframesToPartsCompleted);

                    foreach (string wf in CheckCompletedWarframes(warframesToPartsCompleted))
                    {
                        warframesToPartsCompleted.Remove(wf);

                        Dictionary<string, ItemOrder[]> realOutput = new Dictionary<string, ItemOrder[]>();

                        foreach (string p in Utils.GetPartNames(wf))
                        {
                            realOutput.Add(p, output[p]);
                        }

                        yield return (wf, realOutput);
                    }
                }

                Task<(string item, ItemOrder[] orders)> task = Task.Run(async () => {
                    WebResponse response = await WFWebRequestPresets.GetItemOrders(part).SendAndGetReponseAsync();

                    string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (json.Length <= 50)
                        return (part, Array.Empty<ItemOrder>());

                    JObject parsedResponse = JObject.Parse(json);
                    Dictionary<string, object>[] orders = parsedResponse.SelectToken("payload").SelectToken("orders").ToObject<Dictionary<string, object>[]>();

                    return (part, orders.Select((Dictionary<string, object> raw) => new ItemOrder(raw)).ToArray());
                });

                activeRequests.Add(task);
            }

            while (activeRequests.Count > 0)
            {
                WaitForRequestsToFinish(activeRequests, output, warframesToPartsCompleted);

                foreach (string wf in CheckCompletedWarframes(warframesToPartsCompleted))
                {
                    warframesToPartsCompleted.Remove(wf);

                    Dictionary<string, ItemOrder[]> realOutput = new Dictionary<string, ItemOrder[]>();

                    foreach (string p in Utils.GetPartNames(wf))
                    {
                        realOutput[p] = output[p];
                    }

                    yield return (wf, realOutput);
                }
            }
        }

        private static void WaitForRequestsToFinish(List<Task<(string item, ItemOrder[] orders)>> activeRequests, Dictionary<string, ItemOrder[]> output, Dictionary<string, int> warframesToPartsCompleted)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!activeRequests.Any((Task<(string part, ItemOrder[] orders)> task) => task.IsCompleted))
            {
                ;
            }

            foreach (Task<(string item, ItemOrder[] orders)> req in activeRequests)
            {
                if (req.IsCompleted)
                    output[req.Result.item] = req.Result.orders;
            }

            activeRequests.RemoveAll((Task<(string item, ItemOrder[] orders)> task) => {
                // Spaghetti
                if (task.IsCompleted)
                {
                    string key = Utils.PrimeWarframeNames.First((s) => s.StartsWith(task.Result.item.Split('_')[0]));
                    if (!warframesToPartsCompleted.ContainsKey(key))
                        warframesToPartsCompleted.Add(key, 0);

                    warframesToPartsCompleted[key] += 1;
                    return true;
                }

                return false;
            });

            stopwatch.Stop();
            int ms = (int)stopwatch.ElapsedMilliseconds;

            if (ms < 1000)
                Thread.Sleep(1000 - ms);
        }

        private static IEnumerable<string> CheckCompletedWarframes(Dictionary<string, int> warframesToPartsCompleted) => warframesToPartsCompleted.Keys.Where((s) => warframesToPartsCompleted[s] >= 5);
    }
}
