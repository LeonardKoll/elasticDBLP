using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.SignalR;

namespace dblp
{
    public class ElasticConnector : Hub
    {
        public static ConcurrentDictionary<String, DateTime> last_answered_query = new ConcurrentDictionary<string, DateTime>();

        public override async Task OnConnectedAsync()
        {
            last_answered_query.TryAdd(Context.ConnectionId, DateTime.Now);
            await base.OnConnectedAsync();
        }

        public void Search(string term)
        {
            DateTime queryReceived = DateTime.Now;

            // Trigger Elasticsearch
            // TODO

            Clients.Caller.SendAsync("ReceiveResults", File.ReadAllText("sampleresult.txt"));
            // Deliver
            if (queryReceived > last_answered_query[Context.ConnectionId])
            {
                last_answered_query[Context.ConnectionId] = queryReceived;
                Clients.Caller.SendAsync("ReceiveResults", File.ReadAllText("sampleresult.txt"));
            }
        }

        public async override Task OnDisconnectedAsync(Exception ex)
        {
            DateTime temp;
            last_answered_query.TryRemove(Context.ConnectionId, out temp);
            await base.OnConnectedAsync();
        }
    }
}
