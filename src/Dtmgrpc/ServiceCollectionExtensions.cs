using DtmCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

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
            // dtm driver
            services.TryAddSingleton<Driver.IDtmDriver, Driver.DefaultDtmDriver>();

            // trans releate
            services.AddSingleton<IDtmTransFactory, DtmTransFactory>();
            services.AddSingleton<IDtmgRPCClient, DtmgRPCClient>();
            services.AddSingleton<TccGlobalTransaction>();

            DtmCommon.ServiceCollectionExtensions.AddDtmCommon(services);

            // barrier factory
            services.AddSingleton<IBranchBarrierFactory, DefaultBranchBarrierFactory>();

            return services;
        }
    }
}
