using Dtmgrpc.DtmGImp;
using Google.Protobuf;
using Grpc.Core;

namespace Dtmgrpc
{
    public interface IDtmgRPCClient
    {
        Task DtmGrpcCall(TransBase transBase, string operation);

        Task<string> GenGid(string grpcServer);

        TransBase TransBaseFromGrpc(ServerCallContext context);

        Task RegisterBranch(TransBase tb, string branchId, ByteString bd, Dictionary<string, string> added, string operation);

        Task<TResponse> InvokeBranch<TRequest, TResponse>(TransBase tb, TRequest msg, string url, string branchId, string op)
            where TRequest : class, IMessage, new()
            where TResponse : class, IMessage, new();
    }
}
