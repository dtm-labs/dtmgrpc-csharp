using busi;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Text.Json;

namespace BusiGrpcService.Services
{
    public class BusiApiService : Busi.BusiBase
    {

        private readonly ILogger<BusiApiService> _logger;
        private readonly Dtmgrpc.IDtmgRPCClient _client;

        public BusiApiService(ILogger<BusiApiService> logger, Dtmgrpc.IDtmgRPCClient client)
        { 
            this._logger = logger;
            this._client = client;
        }

        public override async Task<Empty> TransIn(BusiReq request, ServerCallContext context)
        {
            _logger.LogInformation("TransIn req={req}", JsonSerializer.Serialize(request));

            if (string.IsNullOrWhiteSpace(request.TransInResult) || request.TransInResult.Equals("SUCCESS"))
            {
                await Task.CompletedTask;
                return new Empty();
            }
            else if (request.TransInResult.Equals("FAILURE"))
            {
                throw new Grpc.Core.RpcException(new Status(StatusCode.Aborted, "FAILURE"));
            }
            else if (request.TransInResult.Equals("ONGOING"))
            {
                throw new Grpc.Core.RpcException(new Status(StatusCode.FailedPrecondition, "ONGOING"));
            }

            throw new Grpc.Core.RpcException(new Status(StatusCode.Internal, $"unknow result {request.TransInResult}"));
        }

        public override async Task<Empty> TransInTcc(BusiReq request, ServerCallContext context)
        {
            _logger.LogInformation("TransIn req={req}", JsonSerializer.Serialize(request));

            if (string.IsNullOrWhiteSpace(request.TransInResult) || request.TransInResult.Equals("SUCCESS"))
            {
                await Task.CompletedTask;
                return new Empty();
            }
            else if (request.TransInResult.Equals("FAILURE"))
            {
                throw new Grpc.Core.RpcException(new Status(StatusCode.Aborted, "FAILURE"));
            }
            else if (request.TransInResult.Equals("ONGOING"))
            {
                throw new Grpc.Core.RpcException(new Status(StatusCode.FailedPrecondition, "ONGOING"));
            }

            throw new Grpc.Core.RpcException(new Status(StatusCode.Internal, $"unknow result {request.TransInResult}"));
        }

        public override async Task<Empty> TransInConfirm(BusiReq request, ServerCallContext context)
        {
            var tb = _client.TransBaseFromGrpc(context);

            _logger.LogInformation("TransInConfirm tb={tb}, req={req}", JsonSerializer.Serialize(tb), JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<Empty> TransInRevert(BusiReq request, ServerCallContext context)
        {
            var tb = _client.TransBaseFromGrpc(context);

            _logger.LogInformation("TransInRevert tb={tb}, req={req}", JsonSerializer.Serialize(tb), JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<Empty> TransOut(BusiReq request, ServerCallContext context)
        {
            _logger.LogInformation("TransOut req={req}", JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<Empty> TransOutTcc(BusiReq request, ServerCallContext context)
        {
            _logger.LogInformation("TransOut req={req}", JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<Empty> TransOutConfirm(BusiReq request, ServerCallContext context)
        {
            var tb = _client.TransBaseFromGrpc(context);

            _logger.LogInformation("TransOutConfirm tb={tb}, req={req}", JsonSerializer.Serialize(tb), JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<Empty> TransOutRevert(BusiReq request, ServerCallContext context)
        {
            var tb = _client.TransBaseFromGrpc(context);

            _logger.LogInformation("TransOutRevert tb={tb}, req={req}", JsonSerializer.Serialize(tb), JsonSerializer.Serialize(request));
            await Task.CompletedTask;
            return new Empty();
        }

        public override async Task<BusiReply> QueryPrepared(BusiReq request, ServerCallContext context)
        {
            var tb = _client.TransBaseFromGrpc(context);

            _logger.LogInformation("TransOutRevert tb={tb}, req={req}", JsonSerializer.Serialize(tb), JsonSerializer.Serialize(request));

            Exception ex = null;

            if (request.TransInResult.Contains("qp-yes") || string.IsNullOrWhiteSpace(request.TransInResult))
            {
                await Task.CompletedTask;
                return new BusiReply { Message = "a sample data" };
            }
            else if(request.TransInResult.Contains("qp-failure"))
            {
                ex = Dtmgrpc.DtmGImp.Utils.String2DtmError("FAILURE");
            }
            else if (request.TransInResult.Contains("qp-ongoing"))
            {
                ex = Dtmgrpc.DtmGImp.Utils.String2DtmError("ONGOING");
            }

            throw Dtmgrpc.DtmGImp.Utils.DtmError2GrpcError(ex);
        }
    }   
}