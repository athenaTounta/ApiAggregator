
namespace ApiAggregator.Application.Abstractions
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan? absoluteTimeExpiration = null);
        bool TryGet<T>(string key, out T value);
    }
}
