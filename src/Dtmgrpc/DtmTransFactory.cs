using Microsoft.Extensions.Options;

namespace Dtmgrpc
{
    public class DtmTransFactory : IDtmTransFactory
    {
        private readonly DtmOptions _options;
        private readonly IDtmgRPCClient _rpcClient;

        public DtmTransFactory(IOptions<DtmOptions> optionsAccs, IDtmgRPCClient rpcClient)
        { 
            _options = optionsAccs.Value;
            _rpcClient = rpcClient;
        }

        public MsgGrpc NewMsgGrpc(string gid)
        {
            var msg = new MsgGrpc(_rpcClient, _options.DtmGrpcUrl, gid);
            return msg;
        }

        public SagaGrpc NewSagaGrpc(string gid)
        {
            var saga = new SagaGrpc(_rpcClient, _options.DtmGrpcUrl, gid);
            return saga;
        }
    }
}
