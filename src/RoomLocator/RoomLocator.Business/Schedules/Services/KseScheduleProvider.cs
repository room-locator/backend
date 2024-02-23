using RoomLocator.Business.Schedules.Interfaces;
using RoomLocator.Persistence.Cache;

namespace RoomLocator.Business.Schedules.Services;

public class KseScheduleProvider
{
    private const string CachedRoomsKey = "rooms";

    private readonly ICacheService _cacheService;
    private readonly IKseScheduleClient _kseScheduleClient;

    public KseScheduleProvider(ICacheService cacheService, IKseScheduleClient kseScheduleClient)
    {
        _cacheService = cacheService;
        _kseScheduleClient = kseScheduleClient;
    }

    public async Task<Dictionary<string, string>> GetIcalContentsByRoomsAsync()
    {
        var rooms = await _kseScheduleClient.GetRoomsAsync();

        var serialized = await _cacheService.GetAsync<List<string>>(CachedRoomsKey);

        if (serialized == null)
        {
            var tasks = new List<Task<string>>();

            foreach (var room in rooms)
            {
                tasks.Add(_kseScheduleClient.GetIcalContentByRoomAsync(room.Id));
            }

            serialized = (await Task.WhenAll(tasks)).ToList();

            await _cacheService.SetAsync(CachedRoomsKey, serialized, TimeSpan.FromMinutes(10));
        }

        var dictionary = new Dictionary<string, string>();

        for (int i = 0; i < rooms.Count; i++)
        {
            dictionary.Add(rooms[i].Label, serialized[i]);
        }

        return dictionary;
    }
}
