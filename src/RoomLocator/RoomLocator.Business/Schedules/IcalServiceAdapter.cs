using RoomLocator.Business.Schedules.Interfaces;

namespace RoomLocator.Business.Schedules;

public class IcalServiceAdapter : IIcalService
{
    private readonly IcalService _icalService;

    public IcalServiceAdapter(IcalService icalService)
    {
        _icalService = icalService;
    }

    public List<TimeRange> Deserialize(string content)
    {
        var calendar = _icalService.Deserialize(content);

        var ranges = new List<TimeRange>();

        foreach (var range in calendar.Events)
        {
            var dtStart = range.DtStart;
            var dtEnd = range.DtEnd;

            ranges.Add(new TimeRange
            {
                Type = TimeRangeTypes.Busy,
                To = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour, dtEnd.Minute, dtEnd.Second),
                From = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, dtStart.Minute, dtStart.Second),
            });
        }

        return ranges;
    }
}
