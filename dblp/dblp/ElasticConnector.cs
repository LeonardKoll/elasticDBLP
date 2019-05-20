using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.SignalR;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace dblp
{
    public class ElasticConnector : Hub
    {
        private static ConcurrentDictionary<String, DateTime> last_answered_query = new ConcurrentDictionary<string, DateTime>();

        public override async Task OnConnectedAsync()
        {
            last_answered_query.TryAdd(Context.ConnectionId, DateTime.Now);
            await base.OnConnectedAsync();
        }

        public void Search(string term)
        {
            DateTime queryReceived = DateTime.Now;

            // Trigger Elastic
            JObject response = ElasticSearch(term);

            // Deliver
            if (queryReceived > last_answered_query[Context.ConnectionId])
            {   // If a later query has already been answered, we don't need to send this one.

                last_answered_query[Context.ConnectionId] = queryReceived;

                // Convert Results
                JArray results = (JArray)response["hits"]["hits"];
                if (results.HasValues)
                {
                    results = JArray.FromObject(results
                        .Select(c => c["_source"])
                        .ToList()
                    );
                }
                
                // Send
                Clients.Caller.SendAsync("ReceiveResults", results.ToString());
            }
        }

        public async override Task OnDisconnectedAsync(Exception ex)
        {
            DateTime temp;
            last_answered_query.TryRemove(Context.ConnectionId, out temp);
            await base.OnConnectedAsync();
        }

        private JObject ElasticSearch(string term)
        {
            var responseString = "http://localhost:9200/dblp/_search".PostJsonAsync( new{
                query = new
                {
                    multi_match = new
                    {
                        query = term
                    }
                }
            }).ReceiveString().Result;
            return JObject.Parse(responseString);
        }
    }
}
