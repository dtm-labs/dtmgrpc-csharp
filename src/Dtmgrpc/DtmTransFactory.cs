using DtmCommon;
using Microsoft.Extensions.Options;

namespace Dtmgrpc
{
    public class DtmTransFactory : IDtmTransFactory
    {
        private readonly DtmOptions _options;
        private readonly IDtmgRPCClient _rpcClient;
        private readonly IBranchBarrierFactory _branchBarrierFactory;

        public DtmTransFactory(IOptions<DtmOptions> optionsAccs, IDtmgRPCClient rpcClient, IBranchBarrierFactory branchBarrierFactory)
        {
            this._options = optionsAccs.Value;
            this._rpcClient = rpcClient;
            this._branchBarrierFactory = branchBarrierFactory;
        }

        public MsgGrpc NewMsgGrpc(string gid)
        {
            var msg = new MsgGrpc(_rpcClient, _branchBarrierFactory, _options.DtmGrpcUrl, gid);
            return msg;
        }

        public SagaGrpc NewSagaGrpc(string gid)
        {
            var saga = new SagaGrpc(_rpcClient, _options.DtmGrpcUrl, gid);
            return saga;
        }
    }
}
