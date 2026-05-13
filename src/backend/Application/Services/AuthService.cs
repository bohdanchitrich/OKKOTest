using Application.Abstractions.Repositories;
using Application.Abstractions.Security;
using Application.Exceptions;
using Application.Services.REQMs;
using Application.Services.RESMs;
using Domain.Entities;
using Mapster;

namespace Application.Services
{
    public class AuthService
    {
        private readonly IRepository<FullTokenEntity> _fullTokenRepository;
        private readonly ISimpleTokenRepository _simpleTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHashProvider _hashProvider;

        public AuthService(IRepository<FullTokenEntity> fullTokenRepository, ISimpleTokenRepository simpleTokenRepository, IUserRepository userRepository, ITokenProvider tokenProvider, IHashProvider hashProvider)
        {
            _fullTokenRepository = fullTokenRepository;
            _simpleTokenRepository = simpleTokenRepository;
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
            _hashProvider = hashProvider;
        }

        public async Task<LoginRESM?> LoginAsync(LoginREQM requestModel)
        {
            var user = await _userRepository.GetByLoginAsync(requestModel.Login);

            if (user is null)
                throw new UnauthorizedException("Invalid credentials.");

            var verified = _hashProvider.Verify(
                requestModel.Password,
                user.PasswordHash);

            if (!verified)
                throw new UnauthorizedException("Invalid credentials.");


            var simpleToken = _tokenProvider.CreateSimpleToken(user.Login);

            await _simpleTokenRepository.AddAsync(simpleToken);

            return simpleToken.Adapt<LoginRESM>();
        }

        public async Task<ExchangeTokenRESM?> ExchangeTokenAsync(ExchangeTokenREQM requestModel)
        {

            var consumedToken = await _simpleTokenRepository.ConsumeAsync(requestModel.SimpleToken);

            if (consumedToken is null)
            {
                throw new UnauthorizedException("Invalid token.");
            }
            var tokenValid = _tokenProvider.ValidateSimpleToken(consumedToken);
            if (!tokenValid)
            {
                throw new UnauthorizedException("Invalid token.");
            }
            var fullToken = _tokenProvider.CreateFullToken(consumedToken.UserLogin);

            fullToken = await _fullTokenRepository.AddAsync(fullToken);

            return fullToken.Adapt<ExchangeTokenRESM>();
        }

        public async Task<bool> LogoutAsync(LogoutREQM reqeustModel)
        {

            var existingToken = await _fullTokenRepository.FirstOrDefaultAsync(t => t.Value ==  reqeustModel.FullToken);
            if (existingToken == null)
            {
                throw new UnauthorizedException("Invalid token.");
            }
            return await _fullTokenRepository.DeleteAsync(existingToken);
        }
    }
}
