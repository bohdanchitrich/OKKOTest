using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Storage;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    internal class SimpleTokenRepository : Repository<SimpleTokenEntity>,ISimpleTokenRepository
    {
        public SimpleTokenRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public Task<SimpleTokenEntity?> ConsumeAsync(string tokenValue)
        {
            dictionary.TryRemove(tokenValue, out var token);

            return Task.FromResult(token);
        }

    }
}
