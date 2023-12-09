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
            // convert CalendarEvents to TimeRange
        }
        
        return ranges;
    }
}
