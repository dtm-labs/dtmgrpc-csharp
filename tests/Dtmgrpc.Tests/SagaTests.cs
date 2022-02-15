using Google.Protobuf.WellKnownTypes;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Dtmgrpc.Tests
{
    public class SagaTests
    {
        private static readonly string busi = "localhost:8081/busisvc";

        [Fact]
        public async void Submit_Should_Succeed()
        {
            var dtmClient = new Mock<IDtmgRPCClient>();
            TransMockHelper.MockTransCallDtm(dtmClient, Constant.Op.Submit, false);
            
            var gid = "TestSagaNormal";
            var saga = new SagaGrpc(dtmClient.Object, "", gid);

            var req = new Empty();

            saga.Add(string.Concat(busi, "/TransOut"), string.Concat(busi, "/TransOutRevert"), req)
                .Add(string.Concat(busi, "/TransOut"), string.Concat(busi, "/TransOutRevert"), req)
                .Add(string.Concat(busi, "/TransIn"), string.Concat(busi, "/TransInRevert"), req)
                .Add(string.Concat(busi, "/TransIn"), string.Concat(busi, "/TransInRevert"), req)
                .EnableWaitResult()
                .SetRetryInterval(10)
                .SetTimeoutToFail(100)
                .SetBranchHeaders(new Dictionary<string, string>
                 {
                     { "bh1", "123" },
                     { "bh2", "456" },
                 });

            await saga.Submit();

            Assert.True(true);
        }

        [Fact]
        public async void Submit_Should_ThrowException()
        {
            var dtmClient = new Mock<IDtmgRPCClient>();
            TransMockHelper.MockTransCallDtm(dtmClient, Constant.Op.Submit, true);

            var gid = "TestSagaNormal";
            var saga = new SagaGrpc(dtmClient.Object, "", gid);

            var req = new Empty();

            saga.Add(string.Concat(busi, "/TransOut"), string.Concat(busi, "/TransOutRevert"), req)
                .Add(string.Concat(busi, "/TransIn"), string.Concat(busi, "/TransInRevert"), req);

            await Assert.ThrowsAnyAsync<Exception>(async () => await saga.Submit());
        }
    }
}
