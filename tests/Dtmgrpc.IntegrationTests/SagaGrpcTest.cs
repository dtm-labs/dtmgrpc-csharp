using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dtmgrpc.IntegrationTests
{
    public class SagaGrpcTest
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

            var gid = "sagaTestGid" + Guid.NewGuid().ToString();

            var saga = transFactory.NewSagaGrpc(gid);
            var req = new busi.BusiReq { Amount = 30, TransInResult = "", TransOutResult = "" };
            var busiGrpc = "localhost:5005";

            saga.Add(busiGrpc + "/busi.Busi/TransOut", busiGrpc + "/busi.Busi/TransOutRevert", req);
            saga.Add(busiGrpc + "/busi.Busi/TransIn", busiGrpc + "/busi.Busi/TransInRevert", req);
            await saga.Submit();

            Assert.True(true);
        }
    }
}
