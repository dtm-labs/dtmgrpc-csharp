using Dtmgrpc.DtmGImp;
using Google.Protobuf;

namespace Dtmgrpc
{
    public class MsgGrpc
    {
        private static readonly string Action = "action";

        private readonly TransBase _transBase;
        private readonly IDtmgRPCClient _dtmClient;

        public MsgGrpc(IDtmgRPCClient dtmHttpClient, string server, string gid)
        {
            this._dtmClient = dtmHttpClient;
            this._transBase = TransBase.NewTransBase(gid, Constant.TYPE_MSG, server, string.Empty);
        }

        public MsgGrpc Add(string action, IMessage payload)
        {
            if (this._transBase.Steps == null) this._transBase.Steps = new List<Dictionary<string, string>>();
            if (this._transBase.BinPayloads == null) this._transBase.BinPayloads = new List<byte[]>();

            this._transBase.Steps.Add(new Dictionary<string, string> { { Action, action } });
            this._transBase.BinPayloads.Add(payload.ToByteArray());
            return this;
        }

        public async Task Prepare(string queryPrepared, CancellationToken cancellationToken = default)
        {
            this._transBase.QueryPrepared = !string.IsNullOrWhiteSpace(queryPrepared) ? queryPrepared : this._transBase.QueryPrepared;

            await this._dtmClient.DtmGrpcCall(this._transBase, Constant.Op.Prepare);
        }

        public async Task Submit(CancellationToken cancellationToken = default)
        {
            await this._dtmClient.DtmGrpcCall(this._transBase, Constant.Op.Submit);
        }

        /// <summary>
        /// Enable wait result for trans
        /// </summary>
        /// <returns></returns>
        public MsgGrpc EnableWaitResult()
        {
            this._transBase.WaitResult = true;
            return this;
        }

        /// <summary>
        /// Set timeout to fail for trans, unit is second
        /// </summary>
        /// <param name="timeoutToFail">timeout to fail</param>
        /// <returns></returns>
        public MsgGrpc SetTimeoutToFail(long timeoutToFail)
        {
            this._transBase.TimeoutToFail = timeoutToFail;
            return this;
        }

        /// <summary>
        /// Set retry interval for trans, unit is second
        /// </summary>
        /// <param name="retryInterval"></param>
        /// <returns></returns>
        public MsgGrpc SetRetryInterval(long retryInterval)
        {
            this._transBase.RetryInterval = retryInterval;
            return this;
        }

        /// <summary>
        /// Set branch headers for trans
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public MsgGrpc SetBranchHeaders(Dictionary<string, string> headers)
        {
            this._transBase.BranchHeaders = headers;
            return this;
        }
    }
}
