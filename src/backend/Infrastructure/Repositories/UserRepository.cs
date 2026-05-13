using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Storage;

namespace Infrastructure.Repositories
{
    internal class UserRepository : Repository<UserEntity>, IUserRepository
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _applicationContext = applicationContext;
        }


        public Task<UserEntity?> GetByLoginAsync(string login)
        {
            _applicationContext.Users.TryGetValue(login, out var userEntity);
            return Task.FromResult(userEntity);
        }



    }
}
