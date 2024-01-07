using RoomLocator.Business.Schedules.Interfaces;
using RoomLocator.Business.Schedules.Providers.Kse;

namespace RoomLocator.Business.Schedules;

public class ScheduleService
{
    private readonly IIcalService _icalService;
    private readonly KseScheduleProvider _kseScheduleProvider;

    public ScheduleService(IIcalService icalService, KseScheduleProvider kseScheduleProvider)
    {
        _icalService = icalService;
        _kseScheduleProvider = kseScheduleProvider;
    }

    public async Task<List<RichRoom>> FindAvailableRoomsAsync(DateTime? desiredTime = null)
    {
        if (desiredTime.HasValue && desiredTime.Value < DateTime.Now)
        {
            throw new Exception("desiredTime can not be less or equal to current date");
        }

        desiredTime ??= DateTime.Now;
        
        var dictionary = await _kseScheduleProvider.GetIcalContentsByRooms();

        var richRooms = new List<RichRoom>();

        foreach (var entry in dictionary)
        {
            var timeRanges = _icalService.Deserialize(entry.Value);

            var schedule = new Schedule
            {
                Room = entry.Key,
                TimeRanges = timeRanges,
            };

            if (!Available(desiredTime.Value, schedule))
            {
                continue;
            }

            var nearestFutureRange = CalculateNearestFutureRange(desiredTime.Value, schedule);

            if (OutOfRange(desiredTime.Value, nearestFutureRange))
            {
                continue;
            }

            richRooms.Add(new RichRoom
                {
                    Name = entry.Key.Name,
                    ExternalId = entry.Key.ExternalId,
                    NearestTimeRange = nearestFutureRange,
                }
            );
        }

        return richRooms;
    }

    private bool Available(DateTime desiredTime, Schedule schedule)
    {
        foreach (var timeRange in schedule.TimeRanges)
        {
            if (timeRange.From <= desiredTime && desiredTime <= timeRange.To)
            {
                return false;
            }
        }

        return true;
    }

    private bool OutOfRange(DateTime desiredTime, TimeRange? nearestTimeRange)
    {
        if (nearestTimeRange is null)
        {
            return false;
        }
        
        if (nearestTimeRange.From.Date != desiredTime.Date)
        {
            return true;
        }

        return false;
    }

    private TimeRange CalculateNearestFutureRange(DateTime desiredTime, Schedule schedule)
    {
        TimeRange nearestFutureTimeRange = null;

        foreach (var timeRange in schedule.TimeRanges)
        {
            if (timeRange.From <= desiredTime && desiredTime <= timeRange.To)
            {
                continue;
            }

            if (desiredTime >= timeRange.From)
            {
                continue;
            }

            nearestFutureTimeRange ??= timeRange;

            if (timeRange.From - desiredTime < nearestFutureTimeRange.From - desiredTime)
            {
                nearestFutureTimeRange = timeRange;
            }
        }

        return nearestFutureTimeRange;
    }
}
