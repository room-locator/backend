namespace RoomLocator.Business.Schedules;

public class ScheduleService
{
    public (bool available, TimeRange? nearestFutureTimeRange) CheckAvailability(DateTime desiredTime,
        Schedule schedule)
    {
        TimeRange nearestFutureTimeRange = null;

        foreach (var timeRange in schedule.TimeRanges)
        {
            nearestFutureTimeRange ??= timeRange;

            if (timeRange.From <= desiredTime && desiredTime <= timeRange.To)
            {
                return (false, null);
            }

            if (timeRange.From - desiredTime < nearestFutureTimeRange.From - desiredTime)
            {
                nearestFutureTimeRange = timeRange;
            }
        }

        return (true, nearestFutureTimeRange);
    }
}
