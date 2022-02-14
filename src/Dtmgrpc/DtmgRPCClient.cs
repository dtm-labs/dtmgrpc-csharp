using Dtmgrpc.DtmGImp;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace Dtmgrpc
{
    public class DtmgRPCClient : IDtmgRPCClient
    {
        private static readonly Marshaller<dtmgpb.DtmRequest> DtmRequestMarshaller = Marshallers.Create(r => r.ToByteArray(), data => dtmgpb.DtmRequest.Parser.ParseFrom(data));
        private static readonly Marshaller<Empty> DtmReplyMarshaller = Marshallers.Create(r => r.ToByteArray(), data => Empty.Parser.ParseFrom(data));
        private static readonly string DtmServiceName = "dtmgimp.Dtm";

        private readonly Driver.IDtmDriver _dtmDriver;

        public DtmgRPCClient(Driver.IDtmDriver dtmDriver)
        { 
            this._dtmDriver = dtmDriver;
        }

        public async Task DtmGrpcCall(TransBase transBase, string operation)
        {
            var dtmRequest = BuildDtmRequest(transBase);
            var method = new Method<dtmgpb.DtmRequest, Empty>(MethodType.Unary, DtmServiceName, operation, DtmRequestMarshaller, DtmReplyMarshaller);

            using var channel = GrpcChannel.ForAddress(transBase.Dtm);
            await channel.CreateCallInvoker().AsyncUnaryCall(method, string.Empty, new CallOptions(), dtmRequest);
        }

        public async Task<string> GenGid(string grpcServer)
        {
            using var channel = GrpcChannel.ForAddress(grpcServer);
            var client = new dtmgpb.Dtm.DtmClient(channel);
            var reply = await client.NewGidAsync(new Empty(), new CallOptions());
            return reply.Gid;
        }

        public async Task<TResponse> InvokeBranch<TRequest, TResponse>(TransBase tb, TRequest msg, string url, string branchId, string op)
            where TRequest : class, IMessage, new()
            where TResponse : class, IMessage, new()
        {
            var (server, serviceName, method, err) = _dtmDriver.ParseServerMethod(url);

            if(!string.IsNullOrWhiteSpace(err)) throw new Exception(err);

            using var channel = GrpcChannel.ForAddress(server);
            var grpcMethod = Utils.CreateMethod<TRequest, TResponse>(MethodType.Unary, serviceName, method);

            var metadata = Utils.TransInfo2Metadata(tb.Gid, tb.TransType, branchId, op, tb.Dtm);
            var callOptions = new CallOptions();
            callOptions.WithHeaders(metadata);

            var resp = await channel.CreateCallInvoker().AsyncUnaryCall(grpcMethod, string.Empty, callOptions, msg);
            return resp;
        }

        public async Task RegisterBranch(TransBase tb, string branchId, ByteString bd, Dictionary<string, string> added, string operation)
        {
            var request = new dtmgpb.DtmBranchRequest
            {
                Gid = tb.Gid,
                TransType = tb.TransType,
            };

            request.BranchID = branchId;
            request.BusiPayload = bd;
            request.Data.Add(added);

            using var channel = GrpcChannel.ForAddress(tb.Dtm);
            var client = new dtmgpb.Dtm.DtmClient(channel);
            await client.RegisterBranchAsync(request, new CallOptions());
        }

        public TransBase TransBaseFromGrpc(ServerCallContext context)
        {
            var gid = Utils.DtmGet(context, Constant.Md.Gid);
            var transType = Utils.DtmGet(context, Constant.Md.TransType);
            var dtm = Utils.DtmGet(context, Constant.Md.Dtm);
            var branchId = Utils.DtmGet(context, Constant.Md.BranchId);
            var op = Utils.DtmGet(context, Constant.Md.Op);

            var tb = TransBase.NewTransBase(gid, transType, dtm, branchId);
            tb.Op = op;

            return tb;
        }

        private dtmgpb.DtmRequest BuildDtmRequest(TransBase transBase)
        {
            var transOptions = new dtmgpb.DtmTransOptions
            {
                WaitResult = transBase.WaitResult,
                TimeoutToFail = transBase.TimeoutToFail,
                RetryInterval = transBase.RetryInterval,
            };

            transOptions.BranchHeaders.Add(transBase.BranchHeaders);
            transOptions.PassthroughHeaders.Add(transBase.PassthroughHeaders);

            var dtmRequest = new dtmgpb.DtmRequest
            {
                Gid = transBase.Gid,
                TransType = transBase.TransType,
                TransOptions = transOptions,
                QueryPrepared = transBase.QueryPrepared,
                CustomedData = transBase.CustomData,
                Steps = Utils.ToJsonString(transBase.Steps),
            };

            foreach (var item in transBase.BinPayloads)
            {
                dtmRequest.BinPayloads.Add(ByteString.CopyFrom(item));
            }

            return dtmRequest;
        }
    }
}
