using Ical.Net;

namespace RoomLocator.Business.Schedules;

public class IcalService
{
    public Calendar? Deserialize(string content)
    {
        try
        {
            return Calendar.Load(content);
        }
        catch
        {
            return null;
        }
    }
}
