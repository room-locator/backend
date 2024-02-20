using RoomLocator.Business.Schedules.Core;

namespace RoomLocator.Business.Schedules.Interfaces;

public interface IKseScheduleClient
{
    Task<List<KseRoom>> GetRoomsAsync();

    Task<string> GetIcalContentByRoomAsync(int id);
}
