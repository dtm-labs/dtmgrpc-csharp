using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dtmgrpc.IntegrationTests
{
    public class TccGrpcTest
    {
        [Fact]
        public async Task Execute_Should_Succeed()
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

            var globalTransaction = provider.GetRequiredService<TccGlobalTransaction>();

            var gid = "tccTestGid" + Guid.NewGuid().ToString();

            var req = new busi.BusiReq { Amount = 30, TransInResult = "", TransOutResult = "" };
            var busiGrpc = "localhost:5005";
            var res = await globalTransaction.Excecute(dtm, gid, async tcc =>
            {
                await tcc.CallBranch<busi.BusiReq, Empty>(req, busiGrpc + "/busi.Busi/TransOut", busiGrpc + "/busi.Busi/TransOutConfirm", busiGrpc + "/busi.Busi/TransOutRevert");
                await tcc.CallBranch<busi.BusiReq, Empty>(req, busiGrpc + "/busi.Busi/TransIn", busiGrpc + "/busi.Busi/TransInConfirm", busiGrpc + "/busi.Busi/TransInRevert");
                await Task.CompletedTask;
            });

            Assert.Equal(gid, res);

        }
    }
}
