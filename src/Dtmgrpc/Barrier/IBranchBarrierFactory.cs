using Microsoft.Extensions.Logging;

namespace Dtmgrpc
{
    public interface IBranchBarrierFactory
    {
        BranchBarrier CreateBranchBarrier(string transType, string gid, string branchID, string op, ILogger logger = null);

        BranchBarrier CreateBranchBarrier(Grpc.Core.ServerCallContext context, ILogger logger = null);
    }
}