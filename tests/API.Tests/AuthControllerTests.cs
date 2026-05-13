using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;



namespace API.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private const string StaticKey = "super-secret-key";

        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task Login_WithValidCredentials_ReturnsSimpleToken()
        {
            var response = await LoginAsync("admin", "admin");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await ReadJsonAsync(response);
            Assert.IsTrue(json.RootElement.TryGetProperty("simpleToken", out var simpleToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(simpleToken.GetString()));
        }

        [TestMethod]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            var response = await LoginAsync("admin", "wrong-password");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithInvalidSignature_ReturnsUnauthorized()
        {
            var requestDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var response = await _client.PostAsJsonAsync("/auth/login", new
            {
                login = "admin",
                password = "admin",
                requestDate,
                apiSignature = "invalid-signature"
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithExpiredRequestDate_ReturnsUnauthorized()
        {
            var requestDate = DateTimeOffset.UtcNow
                .AddMinutes(-10)
                .ToUnixTimeMilliseconds();

            var response = await _client.PostAsJsonAsync("/auth/login", new
            {
                login = "admin",
                password = "admin",
                requestDate,
                apiSignature = CreateSignature(requestDate)
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Token_WithValidSimpleToken_ReturnsFullToken()
        {
            var simpleToken = await GetSimpleTokenAsync();

            var response = await ExchangeTokenAsync(simpleToken);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await ReadJsonAsync(response);
            Assert.IsTrue(json.RootElement.TryGetProperty("fullToken", out var fullToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(fullToken.GetString()));
        }

        [TestMethod]
        public async Task Token_WithSameSimpleTokenTwice_ReturnsUnauthorized()
        {
            var simpleToken = await GetSimpleTokenAsync();

            var firstResponse = await ExchangeTokenAsync(simpleToken);
            var secondResponse = await ExchangeTokenAsync(simpleToken);

            Assert.AreEqual(HttpStatusCode.OK, firstResponse.StatusCode);
            Assert.AreEqual(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
        }

        [TestMethod]
        public async Task Logout_WithValidFullToken_ReturnsOk()
        {
            var fullToken = await GetFullTokenAsync();

            var response = await LogoutAsync(fullToken);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task Logout_WithUnknownFullToken_ReturnsUnauthorized()
        {
            var response = await LogoutAsync("unknown-full-token");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<HttpResponseMessage> LoginAsync(string login, string password)
        {
            var requestDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return await _client.PostAsJsonAsync("/auth/login", new
            {
                login,
                password,
                requestDate,
                apiSignature = CreateSignature(requestDate)
            });
        }

        private async Task<HttpResponseMessage> ExchangeTokenAsync(string simpleToken)
        {
            var requestDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return await _client.PostAsJsonAsync("/auth/token", new
            {
                simpleToken,
                requestDate,
                apiSignature = CreateSignature(requestDate)
            });
        }

        private async Task<HttpResponseMessage> LogoutAsync(string fullToken)
        {
            var requestDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return await _client.PostAsJsonAsync("/auth/logout", new
            {
                fullToken,
                requestDate,
                apiSignature = CreateSignature(requestDate)
            });
        }

        private async Task<string> GetSimpleTokenAsync()
        {
            var response = await LoginAsync("admin", "admin");
            response.EnsureSuccessStatusCode();

            var json = await ReadJsonAsync(response);

            return json.RootElement
                .GetProperty("simpleToken")
                .GetString()!;
        }

        private async Task<string> GetFullTokenAsync()
        {
            var simpleToken = await GetSimpleTokenAsync();

            var response = await ExchangeTokenAsync(simpleToken);
            response.EnsureSuccessStatusCode();

            var json = await ReadJsonAsync(response);

            return json.RootElement
                .GetProperty("fullToken")
                .GetString()!;
        }

        private static string CreateSignature(long requestDate)
        {
            var raw = $"{StaticKey}{requestDate}";
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }
    }

}
