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

            // barrier database relate
            services.AddSingleton<DtmGImp.IDbSpecial, DtmGImp.MysqlDBSpecial>();
            services.AddSingleton<DtmGImp.IDbSpecial, DtmGImp.PostgresDBSpecial>();
            services.AddSingleton<DtmGImp.IDbSpecial, DtmGImp.SqlServerDBSpecial>();
            services.AddSingleton<DtmGImp.DbSpecialDelegate>();
            services.AddSingleton<DtmGImp.DbUtils>();

            // barrier factory
            services.AddSingleton<IBranchBarrierFactory, DefaultBranchBarrierFactory>();

            return services;
        }
    }
}
