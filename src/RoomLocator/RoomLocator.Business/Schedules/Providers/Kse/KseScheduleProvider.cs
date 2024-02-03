using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules.Providers.Kse;

public class KseScheduleProvider : IScheduleProvider
{
    private readonly IKseScheduleClient _kseScheduleClient;

    public KseScheduleProvider(IKseScheduleClient kseScheduleClient)
    {
        _kseScheduleClient = kseScheduleClient;
    }

    public async Task<Dictionary<string, string>> GetIcalContentsByRooms()
    {
        var rooms = await _kseScheduleClient.GetRoomsAsync();

        var tasks = new List<Task<string>>();

        foreach (var room in rooms)
        {
            tasks.Add(_kseScheduleClient.GetRoomIcalContentAsync(room.Id));
        }

        await Task.WhenAll(tasks);

        var dictionary = new Dictionary<string, string>();

        for (int i = 0; i < rooms.Count; i++)
        {
            dictionary.Add(rooms[i].Label, tasks[i].Result);
        }

        return dictionary;
    }
}
