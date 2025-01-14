using KiloTaxi.API.Helper.Authentication.Interface;

namespace KiloTaxi.API.Helper.ServiceExtensions;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public TokenBlacklistMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklistService)
    {
        _next = next;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task Invoke(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (await _tokenBlacklistService.IsTokenBlacklistedAsync(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted.");
                return;
            }
        }

        await _next(context);
    }
}
