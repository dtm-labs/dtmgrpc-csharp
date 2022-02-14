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
            await Task.CompletedTask;
            return new Empty();
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
    }   
}