using Application.Abstractions.Security;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    internal sealed class SignatureProvider : ISignatureProvider
    {

        private readonly SecurityOptions _options;

        public SignatureProvider(IOptions<SecurityOptions> options)
        {
            _options = options.Value;
        }

        public string Generate(long requestDate)
        {
            var raw = $"{_options.StaticKey}{requestDate}";

            var bytes = Encoding.UTF8.GetBytes(raw);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        public bool Validate(long requestDate, string signature)
        {
            var generatedSignature = Generate(requestDate);

            var generatedBytes = Encoding.UTF8.GetBytes(generatedSignature);

            var providedBytes = Encoding.UTF8.GetBytes(signature);

            return CryptographicOperations.FixedTimeEquals(
                generatedBytes,
                providedBytes);
        }

        public bool IsFresh(long requestDate)
        {
            DateTimeOffset requestTime;

            try
            {
                requestTime = DateTimeOffset.FromUnixTimeMilliseconds(requestDate);
            }
            catch (Exception)
            {
                return false;
            }

            var now = DateTimeOffset.UtcNow;
            var difference = Math.Abs((now - requestTime).TotalMinutes);

            return difference <= _options.SignatureLifetimeMinutes;
        }

    }
}
