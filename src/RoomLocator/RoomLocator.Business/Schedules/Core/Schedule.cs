namespace RoomLocator.Business.Schedules.Core;

public class Schedule
{
    public Room Room { get; set; }
    
    public List<TimeRange> TimeRanges { get; set; }
}
