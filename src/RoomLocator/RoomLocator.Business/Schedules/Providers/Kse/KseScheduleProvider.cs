namespace RoomLocator.Business.Schedules.Providers.Kse;

public class KseScheduleProvider
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
            tasks.Add(_kseScheduleClient.GetIcalContentByRoomAsync(room.Id));
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
