namespace RoomLocator.Business.Schedules.Core;

public class TimeRange
{
    public DateTime From { get; set; }
    
    public DateTime To { get; set; }
    
    public TimeRangeTypes Type { get; set; }
}
