using KiloTaxi.API.Helper.Authentication.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace KiloTaxi.API.Helper.Authentication.Implementation;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _memoryCache;

    public TokenBlacklistService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Adds a token to the blacklist with its expiry time.
    /// </summary>
    public async Task AddTokenToBlacklistAsync(string token, DateTime expiryTime)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expiryTime
        };

        _memoryCache.Set(token, true, cacheEntryOptions);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a token is blacklisted.
    /// </summary>
    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await Task.FromResult(_memoryCache.TryGetValue(token, out _));
    }
}
