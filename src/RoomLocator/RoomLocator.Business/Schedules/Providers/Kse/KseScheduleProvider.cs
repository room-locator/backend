using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules.Providers.Kse;

public class KseScheduleProvider : IScheduleProvider
{
    private readonly HashSet<string> _deadRooms = new HashSet<string>
    {
        "1.08.2 Genesis Classroom",
        "2.08.1 TA Ventures Classroom",
        "2.08.2 TA Ventures Classroom",
        "4.08 TAS Group Classroom",
    };
    
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
            if (_deadRooms.Contains(rooms[i].Label))
            {
                continue;
            }
            
            var room = new Room
            {
                Name = rooms[i].Label,
                ExternalId = rooms[i].Id,
            };

            dictionary.Add(room, tasks[i].Result);
        }

        return dictionary;
    }
}
