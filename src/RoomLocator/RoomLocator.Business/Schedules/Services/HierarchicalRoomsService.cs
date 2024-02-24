using RoomLocator.Business.Schedules.Core;

namespace RoomLocator.Business.Schedules.Services;

public class HierarchicalRoomsService
{
    private readonly Dictionary<string, string> _children = new();

    private readonly Dictionary<string, HashSet<string>> _parents = new()
    {
        { "1.08 Genesis Classroom", new() { "1.08.1 Genesis Classroom", "1.08.2 Genesis Classroom" } },
        {
            "1.17 ASTEM FOUNDATION Classroom",
            new() { "1.17.1 ASTEM FOUNDATION Classroom", "1.17.2 ASTEM FOUNDATION Classroom" }
        },
        { "2.08 TA Ventures Classroom", new() { "2.08.1 TA Ventures Classroom", "2.08.2 TA Ventures Classroom" } },
        {
            "2.15 Roy Gartner Memorial classroom",
            new() { "2.15.1 Roy Gartner Memorial classroom", "2.15.2 Roy Gartner Memorial classroom" }
        }
    };

    public HierarchicalRoomsService()
    {
        foreach (var entry in _parents)
        {
            var children = entry.Value;

            foreach (var child in children)
            {
                _children[child] = entry.Key;
            }
        }
    }

    public List<TimeRange> GetTimeRanges(string name,
        IReadOnlyDictionary<string, List<TimeRange>> deserialized)
    {
        var timeRanges = new List<TimeRange>();

        if (_parents.TryGetValue(name, out var children))
        {
            foreach (var child in children)
            {
                timeRanges.AddRange(deserialized[child]);
            }

            return timeRanges;
        }

        if (_children.TryGetValue(name, out var parent))
        {
            return deserialized[parent];
        }

        return timeRanges;
    }
}
