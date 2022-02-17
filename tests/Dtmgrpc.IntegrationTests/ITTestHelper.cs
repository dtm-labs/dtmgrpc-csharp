using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dtmgrpc.IntegrationTests
{
    public class ITTestHelper
    {
        public static string DTMHttpUrl = "http://localhost:36789";
        public static string DTMgRPCUrl = "http://localhost:36790";
        public static string BuisgRPCUrl = "localhost:5005";
        private static System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();

        public static async Task<string> GetTranStatus(string gid)
        {
            var resp = await _client.GetAsync($"{DTMHttpUrl}/api/dtmsvr/query?gid={gid}").ConfigureAwait(false);

            if (resp.IsSuccessStatusCode)
            { 
                var content = await resp.Content.ReadAsStringAsync();
                var res = System.Text.Json.JsonSerializer.Deserialize<QueryResult>(content);
                return res.Transaction.Status;
            }

            return string.Empty;
        }

        public class QueryResult
        {
            public class TransBranchStore
            { 
            
            }

            public class TransGlobalStore
            {
                [System.Text.Json.Serialization.JsonPropertyName("status")]
                public string Status { get; set; }
            }


            [System.Text.Json.Serialization.JsonPropertyName("branches")]
            public List<TransBranchStore> Branches { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("transaction")]
            public TransGlobalStore Transaction { get; set; }
        }

        public static busi.BusiReq GenBusiReq(bool outFailed, bool inFailed)
        {
            return new busi.BusiReq
            {
                Amount = 30,
                TransOutResult = outFailed ? "FAILURE" : "",
                TransInResult = inFailed ? "FAILURE" : ""
            };
        }

        public static ServiceProvider AddDtmGrpc()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDtmGrpc(x =>
            {
                x.DtmGrpcUrl = DTMgRPCUrl;
            });

            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
