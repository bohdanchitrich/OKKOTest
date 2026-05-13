using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories
{
    public interface ISimpleTokenRepository : IRepository<SimpleTokenEntity>
    {
        Task<SimpleTokenEntity?> ConsumeAsync(string token);
    }
}
