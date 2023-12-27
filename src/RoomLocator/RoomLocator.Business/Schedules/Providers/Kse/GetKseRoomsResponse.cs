namespace RoomLocator.Business.Schedules.Providers.Kse;

public class GetKseRoomsResponse
{
    public string Status { get; set; }
    
    public List<KseRoom> Result { get; set; }
}
