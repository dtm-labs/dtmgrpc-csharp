using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dtmgrpc.Tests
{
    public class GrpcCallInvokerTests
    {
        private static readonly Marshaller<dtmgpb.DtmGidReply> DtmGidReplyMarshaller = Marshallers.Create(r => r.ToByteArray(), data => dtmgpb.DtmGidReply.Parser.ParseFrom(data));
        private static readonly Marshaller<Empty> EmptyMarshaller = Marshallers.Create(r => r.ToByteArray(), data => Empty.Parser.ParseFrom(data));

        [Fact(Skip = "local")]
        public async Task CallInvokerAsync()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var method = new Method<Empty, dtmgpb.DtmGidReply>(MethodType.Unary, "dtmgimp.Dtm", "NewGid", EmptyMarshaller, DtmGidReplyMarshaller);

            using var channel = GrpcChannel.ForAddress("http://localhost:36790");
            var resp = await channel.CreateCallInvoker().AsyncUnaryCall(method, string.Empty, new CallOptions(), new Empty());

            Assert.NotNull(resp.Gid);
        }
    }
}
