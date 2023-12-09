using Ical.Net;

namespace RoomLocator.Business.Schedules;

public class IcalService 
{
    public Calendar Deserialize(string content)
    {
        return Calendar.Load(content);
    }
}
