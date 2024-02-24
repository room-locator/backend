using RoomLocator.Persistence.Cache;
using RoomLocator.Business.Schedules.Core;
using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules.Services;

public class RoomLocatorService
{
    private readonly IIcalService _icalService;
    private readonly ICacheService _cacheService;
    private readonly KseScheduleProvider _kseScheduleProvider;
    private readonly HierarchicalRoomsService _hierarchicalRoomsService;

    private const string DeserializedCacheKey = "rooms:deserialized";
    private readonly TimeSpan DeserializedCacheTtl = TimeSpan.FromMinutes(5);

    public RoomLocatorService(
        IIcalService icalService,
        ICacheService cacheService,
        KseScheduleProvider kseScheduleProvider,
        HierarchicalRoomsService hierarchicalRoomsService)
    {
        _icalService = icalService;
        _cacheService = cacheService;
        _kseScheduleProvider = kseScheduleProvider;
        _hierarchicalRoomsService = hierarchicalRoomsService;
    }

    public async Task<List<CalculatedRoom>> LocateAsync(DateTime? desiredTime = null)
    {
        desiredTime = ValidateOrCoalesce(desiredTime);

        var calculatedRooms = new List<CalculatedRoom>();

        var deserialized = await GetAndDeserializeAsync();

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
                _hierarchicalRoomsService.GetTimeRanges(schedule.Room.Name, deserialized)
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
            });
        }

        return calculatedRooms;
    }

    private DateTime ValidateOrCoalesce(DateTime? desiredTime)
    {
         if (desiredTime.HasValue && desiredTime.Value < DateTime.Now)
         {
             throw new Exception("desiredTime can not be less or equal to current date");
         }
 
         desiredTime ??= TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Europe/Kiev");       
         
         return desiredTime.Value;
    }

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

        await _cacheService.SetAsync(DeserializedCacheKey, deserialized, DeserializedCacheTtl);

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
