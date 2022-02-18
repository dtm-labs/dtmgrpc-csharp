using DtmCommon;
using Dtmgrpc.DtmGImp;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

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

                await tcc_cb(tcc);

                await _dtmClient.DtmGrpcCall(tcc.GetTransBase(), Constant.Op.Submit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "submitting or abort global transaction error");
                await _dtmClient.DtmGrpcCall(tcc.GetTransBase(), Constant.Op.Abort);
                return string.Empty;
            }

            return gid;
        }
    }
}
