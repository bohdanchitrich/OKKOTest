using Application.Abstractions.Repositories;
using Application.Abstractions.Security;
using Domain.Entities;
using Infrastructure.BackgroundServices;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<SecurityOptions>()
                .Bind(configuration.GetSection("ApiSignature"))
                .ValidateOnStart();

            services.AddSingleton<ApplicationContext>();

            services.AddSingleton<ISignatureProvider, SignatureProvider>();
            services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddSingleton<IHashProvider, HashProvider>();



            services.AddScoped<ISimpleTokenRepository, SimpleTokenRepository>();
            services.AddScoped<IRepository<FullTokenEntity>, Repository<FullTokenEntity>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddHostedService<ExpiredTokenCleanupService>();


            return services;
        }
    }
}
