namespace RoomLocator.Persistence.Cache;

public interface ICacheService
{
    Task<List<T>> GetListAsync<T>(string key);
    
    Task CreateListAsync<T>(string key, List<T> list, TimeSpan ttl);
}
