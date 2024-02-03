using RoomLocator.Business.Schedules.Interfaces;
using RoomLocator.Business.Schedules.Providers.Kse;

namespace RoomLocator.Business.Schedules;

public class ScheduleService
{
    private readonly IIcalService _icalService;
    private readonly KseScheduleProvider _kseScheduleProvider;

    private readonly Dictionary<string, string> _children = new();

    private readonly Dictionary<string, HashSet<string>> _parents = new()
    {
        { "1.08 Genesis Classroom", new() { "1.08.1 Genesis Classroom", "1.08.2 Genesis Classroom" } },
        {
            "1.17 ASTEM FOUNDATION Classroom",
            new() { "1.17.1 ASTEM FOUNDATION Classroom", "1.17.2 ASTEM FOUNDATION Classroom" }
        },
        { "2.08 TA Ventures Classroom", new() { "2.08.1 TA Ventures Classroom", "2.08.2 TA Ventures Classroom" } }
    };

    public ScheduleService(IIcalService icalService, KseScheduleProvider kseScheduleProvider)
    {
        _icalService = icalService;
        _kseScheduleProvider = kseScheduleProvider;

        foreach (var entry in _parents)
        {
            var children = entry.Value;

            foreach (var child in children)
            {
                _children[child] = entry.Key;
            }
        }
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

        var parentTimeRanges = new Dictionary<string, List<TimeRange>>();
        var childrenTimeRanges = new Dictionary<string, List<TimeRange>>();

        foreach (var parent in _parents)
        {
            if (!dictionary.TryGetValue(parent.Key, out var ical))
            {
                continue;
            }

            var timeRanges = _icalService.Deserialize(ical);

            parentTimeRanges.Add(parent.Key, timeRanges);
        }

        foreach (var child in _children)
        {
            if (!dictionary.TryGetValue(child.Key, out var ical))
            {
                continue;
            }

            var timeRanges = _icalService.Deserialize(ical);

            childrenTimeRanges.Add(child.Key, timeRanges);
        }

        foreach (var entry in dictionary)
        {
            var timeRanges = _icalService.Deserialize(entry.Value);

            var room = new Room
            {
                Name = entry.Key,
            };

            var schedule = new Schedule
            {
                Room = room,
                TimeRanges = timeRanges,
            };

            if (_parents.TryGetValue(room.Name, out var children))
            {
                foreach (var child in children)
                {
                    schedule.TimeRanges.AddRange(childrenTimeRanges[child]);
                }
            }

            if (_children.TryGetValue(room.Name, out var parent))
            {
                schedule.TimeRanges.AddRange(parentTimeRanges[parent]);
            }

            if (!Available(desiredTime.Value, schedule))
            {
                continue;
            }

            var nearestFutureRange = CalculateNearestFutureRange(desiredTime.Value, schedule);

            richRooms.Add(new RichRoom
                {
                    Name = entry.Key,
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
