using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundServices
{
    internal sealed class ExpiredTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredTokenCleanupService> _logger;

        public ExpiredTokenCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredTokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();

                var simpleTokenRepository = scope.ServiceProvider
                    .GetRequiredService<ISimpleTokenRepository>();

                var fullTokenRepository = scope.ServiceProvider
                    .GetRequiredService<IRepository<FullTokenEntity>>();

                var now = DateTime.UtcNow;

                var expiredSimpleTokens = await simpleTokenRepository.WhereAsync(t => t.ExpiresAt < now);
                var expiredFullTokens = await fullTokenRepository.WhereAsync(t => t.ExpiresAt < now);
                await simpleTokenRepository.DeleteRangeAsync(expiredSimpleTokens);
                await fullTokenRepository.DeleteRangeAsync(expiredFullTokens);

                if (expiredSimpleTokens.Count > 0 || expiredFullTokens.Count > 0)
                {
                    _logger.LogInformation(
                        "Expired tokens removed. Simple: {SimpleCount}, Full: {FullCount}",
                        expiredSimpleTokens.Count,
                        expiredFullTokens.Count);
                }
            }
        }
    }
}
