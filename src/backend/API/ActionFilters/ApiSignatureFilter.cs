using Application.Abstractions.Security;
using Application.Exceptions;
using Application.Services.REQMs;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.ActionFilters
{
    internal class ApiSignatureFilter : IAsyncActionFilter
    {

        private readonly ISignatureProvider _signatureProvider;

        public ApiSignatureFilter(ISignatureProvider signatureProvider)
        {
            _signatureProvider = signatureProvider;
        }
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var signedRequest = context.ActionArguments.Values
                .OfType<SignREQM>()
                .FirstOrDefault();

            if (signedRequest is null)
            {
                throw new UnauthorizedException("Invalid request signature.");
            }

            if (!_signatureProvider.IsFresh(signedRequest.RequestDate))
            {
                throw new UnauthorizedException("Request expired.");
            }

            if (!_signatureProvider.Validate(
                    signedRequest.RequestDate,
                    signedRequest.ApiSignature))
            {
                throw new UnauthorizedException("Invalid request signature.");
            }

            await next();
        }
    }
}
