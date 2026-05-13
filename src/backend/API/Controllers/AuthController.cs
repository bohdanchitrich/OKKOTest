using API.ActionFilters;
using Application.Services;
using Application.Services.REQMs;
using Application.Services.RESMs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("auth")]
[ServiceFilter(typeof(ApiSignatureFilter))]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(
        AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginRESM>> Login([FromBody] LoginREQM requestModel)
    {
        var result = await _authService.LoginAsync(requestModel);
        return Ok(result);
    }

    [HttpPost("token")]
    public async Task<ActionResult<ExchangeTokenRESM>> ExchangeToken([FromBody] ExchangeTokenREQM requestModel)
    {
        var result = await _authService.ExchangeTokenAsync(requestModel);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout( [FromBody] LogoutREQM requestModel)
    {
        var result = await _authService.LogoutAsync(requestModel);
        return Ok(result);
    }
}