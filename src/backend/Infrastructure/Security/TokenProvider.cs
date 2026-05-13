using Application.Abstractions.Security;
using Domain.Entities;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly SecurityOptions _options;

        public TokenProvider(IOptions<SecurityOptions> options)
        {
            _options = options.Value;
        }

        public FullTokenEntity CreateFullToken(string userLogin)
        {
           var token = new FullTokenEntity
            {
                UserLogin = userLogin,
                Value = Generate(),
                ExpiresAt = DateTime.UtcNow.AddHours(_options.FullTokenLifetimeHours)
            };
            return token;
        }

        public SimpleTokenEntity CreateSimpleToken(string userLogin)
        {
           var token = new SimpleTokenEntity
            {
                UserLogin = userLogin,
                Value = Generate(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_options.SimpleTokenLifetimeMinutes)
            };
            return token;
        }

     

        public bool ValidateFullToken(FullTokenEntity token)
        {
            var isValid = token.ExpiresAt > DateTime.UtcNow;
            return isValid;
        }

        public bool ValidateSimpleToken(SimpleTokenEntity token)
        {
            var isValid = token.ExpiresAt > DateTime.UtcNow;
            return isValid;
        }



        private string Generate()
        {
            Span<byte> bytes = stackalloc byte[32];

            RandomNumberGenerator.Fill(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
