namespace RoomLocator.Business.Schedules;

public class Schedule
{
    public Room Room { get; set; }
    
    public List<TimeRange> TimeRanges { get; set; }
}
