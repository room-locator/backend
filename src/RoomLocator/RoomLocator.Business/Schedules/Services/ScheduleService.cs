using RoomLocator.Persistence.Cache;
using RoomLocator.Business.Schedules.Core;
using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules.Services;

public class ScheduleService
{
    private readonly IIcalService _icalService;
    private readonly ICacheService _cacheService;
    private readonly KseScheduleProvider _kseScheduleProvider;
    private readonly HierarchalRoomsService _hierarchalRoomsService;

    private const string DeserializedCacheKey = "rooms:deserialized";

    public ScheduleService(
        IIcalService icalService,
        ICacheService cacheService,
        KseScheduleProvider kseScheduleProvider,
        HierarchalRoomsService hierarchalRoomsService)
    {
        _icalService = icalService;
        _cacheService = cacheService;
        _kseScheduleProvider = kseScheduleProvider;
        _hierarchalRoomsService = hierarchalRoomsService;
    }

    public async Task<List<CalculatedRoom>> FindAvailableRoomsAsync(DateTime? desiredTime = null)
    {
        if (desiredTime.HasValue && desiredTime.Value < DateTime.Now)
        {
            throw new Exception("desiredTime can not be less or equal to current date");
        }

        desiredTime ??= TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Europe/Kiev");

        var deserialized = await GetAndDeserializeAsync();

        var calculatedRooms = new List<CalculatedRoom>();

        foreach (var entry in deserialized)
        {
            var schedule = new Schedule
            {
                Room = new Room
                {
                    Name = entry.Key,
                },
                TimeRanges = entry.Value,
            };

            schedule.TimeRanges.AddRange(
                _hierarchalRoomsService.GetTimeRanges(schedule.Room.Name, deserialized)
            );

            if (!Available(desiredTime.Value, schedule))
            {
                continue;
            }

            var nearestFutureRange = CalculateNearestFutureRange(desiredTime.Value, schedule);

            calculatedRooms.Add(new CalculatedRoom
                {
                    Name = entry.Key,
                    NearestTimeRange = nearestFutureRange,
                }
            );
        }

        return calculatedRooms;
    }

    // TODO: think of a proper way to cache the data
    private async Task<Dictionary<string, List<TimeRange>>> GetAndDeserializeAsync()
    {
        var deserialized = await _cacheService.GetAsync<Dictionary<string, List<TimeRange>>>(DeserializedCacheKey);

        if (deserialized != null)
        {
            return deserialized;
        }

        var serialized = await _kseScheduleProvider.GetSerializedByRoomsAsync();

        deserialized = new Dictionary<string, List<TimeRange>>();

        foreach (var entry in serialized)
        {
            deserialized[entry.Key] = _icalService.Deserialize(entry.Value);
        }

        await _cacheService.SetAsync(DeserializedCacheKey, deserialized, TimeSpan.FromMinutes(4));

        return deserialized;
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
