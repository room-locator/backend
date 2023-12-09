namespace RoomLocator.Business.Schedules;

public class TimeRange
{
    public DateTime From { get; set; }
    
    public DateTime To { get; set; }
    
    // Does not really make any sense since KSE's iCal only has busy time ranges
    // I still might want to leave it here in order to work with other formats later
    public TimeRangeTypes Type { get; set; }
}
