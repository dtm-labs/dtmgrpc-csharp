using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dtmgrpc.IntegrationTests
{
    public class MsgGrpcTest
    {
        [Fact]
        public async Task Submit_Should_Succeed()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var dtm = "http://localhost:36790";
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDtmGrpc(x =>
            {
                x.DtmGrpcUrl = dtm;
            });

            var provider = services.BuildServiceProvider();

            var transFactory = provider.GetRequiredService<IDtmTransFactory>();

            var gid = "msgTestGid" + Guid.NewGuid().ToString();

            var msg = transFactory.NewMsgGrpc(gid);
            var req = new busi.BusiReq { Amount = 30, TransInResult = "", TransOutResult = "" };
            var busiGrpc = "localhost:5005";
            msg.Add(busiGrpc + "/busi.Busi/TransOut", req)
               .Add(busiGrpc + "/busi.Busi/TransIn", req);

            await msg.Prepare(busiGrpc + "/busi.Busi/QueryPrepared");
            await msg.Submit();

            Assert.True(true);
        }
    }
}
