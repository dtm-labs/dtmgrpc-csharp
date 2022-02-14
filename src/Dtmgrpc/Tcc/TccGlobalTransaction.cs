using Dtmgrpc.DtmGImp;
using Microsoft.Extensions.Logging;

namespace Dtmgrpc
{
    public class TccGlobalTransaction
    {
        private readonly IDtmgRPCClient _dtmClient;
        private readonly ILogger _logger;

        public TccGlobalTransaction(IDtmgRPCClient dtmClient, ILoggerFactory factory)
        {
            this._dtmClient = dtmClient;
            this._logger = factory.CreateLogger<TccGlobalTransaction>();
        }

        public async Task<string> Excecute(string dtm, string gid, Func<TccGrpc, Task> tcc_cb, CancellationToken cancellationToken = default)
        {
            return await Excecute(dtm, gid, x => { }, tcc_cb, cancellationToken);
        }

        public async Task<string> Excecute(string dtm, string gid, Action<TccGrpc> custom, Func<TccGrpc, Task> tcc_cb, CancellationToken cancellationToken = default)
        {
            var tcc = new TccGrpc(this._dtmClient, TransBase.NewTransBase(gid, Constant.TYPE_TCC, dtm, ""));
            custom(tcc);

            try
            {
                await _dtmClient.DtmGrpcCall(tcc.GetTransBase(), Constant.Op.Prepare);
                //logger.LogDebug("prepare result gid={gid}, res={res}", gid, prepare);

                await tcc_cb(tcc);

                await _dtmClient.DtmGrpcCall(tcc.GetTransBase(), Constant.Op.Submit);
                //logger.LogDebug("submit result gid={gid}, res={res}", gid, submit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "submitting or abort global transaction error");
                await _dtmClient.DtmGrpcCall(tcc.GetTransBase(), Constant.Op.Abort);
                //logger.LogDebug("abort result gid={gid}, res={res}", gid, abort);
                return string.Empty;
            }
            return gid;
        }
    }
}
