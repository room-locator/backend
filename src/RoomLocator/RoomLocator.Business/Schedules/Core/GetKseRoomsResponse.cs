namespace RoomLocator.Business.Schedules.Core;

public class GetKseRoomsResponse
{
    public string Status { get; set; }
    
    public List<KseRoom> Result { get; set; }
}
