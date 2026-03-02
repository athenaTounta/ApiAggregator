using ApiAggregator.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace ApiAggregator.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(10);

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Set<T>(string key, T value, TimeSpan? absoluteTimeExpiration = null)
    {
        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteTimeExpiration ?? _defaultExpiration
        });
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_cache.TryGetValue(key, out T? result))
        {
            value = result!;
            return true;
        }
        value = default!;
        return false;
    }
}