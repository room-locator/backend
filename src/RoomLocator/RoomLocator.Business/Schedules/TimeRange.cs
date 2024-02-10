namespace RoomLocator.Business.Schedules;

public class TimeRange
{
    public DateTime From { get; set; }
    
    public DateTime To { get; set; }
    
    public TimeRangeTypes Type { get; set; }
}
