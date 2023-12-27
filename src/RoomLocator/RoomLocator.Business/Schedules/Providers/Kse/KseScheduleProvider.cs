using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules.Providers.Kse;

public class KseScheduleProvider : IScheduleProvider
{
    private readonly IKseScheduleClient _kseScheduleClient;

    public KseScheduleProvider(IKseScheduleClient kseScheduleClient)
    {
        _kseScheduleClient = kseScheduleClient;
    }

    public async Task<Dictionary<Room, string>> GetIcalContentsByRooms()
    {
        var rooms = await _kseScheduleClient.GetRoomsAsync();

        var tasks = new List<Task<string>>();

        foreach (var room in rooms)
        {
            tasks.Add(_kseScheduleClient.GetRoomIcalContentAsync(room.Id));
        }

        await Task.WhenAll(tasks);

        var dictionary = new Dictionary<Room, string>();

        for (int i = 0; i < rooms.Count; i++)
        {
            var room = new Room
            {
                Name = rooms[i].Name,
                ExternalId = rooms[i].Id,
            };

            dictionary.Add(room, tasks[i].Result);
        }

        return dictionary;
    }
}
