using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dtmgrpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDtmGrpc(this IServiceCollection services, Action<DtmOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);

            return AddDtmGrpcCore(services);
        }

        public static IServiceCollection AddDtmGrpc(this IServiceCollection services, IConfiguration configuration, string sectionName = "dtm")
        {
            services.Configure<DtmOptions>(configuration.GetSection(sectionName));

            return AddDtmGrpcCore(services);
        }

        private static IServiceCollection AddDtmGrpcCore(IServiceCollection services)
        {
            services.AddSingleton<Driver.IDtmDriver, Driver.DefaultDtmDriver>();

            services.AddSingleton<IDtmTransFactory, DtmTransFactory>();
            services.AddSingleton<IDtmgRPCClient, DtmgRPCClient>();
            services.AddSingleton<TccGlobalTransaction>();

            //services.AddSingleton<IBranchBarrierFactory, DefaultBranchBarrierFactory>();

            return services;
        }
    }
}
