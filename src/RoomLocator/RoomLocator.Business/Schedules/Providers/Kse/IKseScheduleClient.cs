namespace RoomLocator.Business.Schedules.Providers.Kse;

public interface IKseScheduleClient
{
    Task<List<KseRoom>> GetRoomsAsync();

    Task<string> GetIcalContentByRoomAsync(int id);
}
