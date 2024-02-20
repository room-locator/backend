using RoomLocator.Business.Schedules.Core;

namespace RoomLocator.Business.Schedules.Interfaces;

public interface IIcalService
{
    List<TimeRange> Deserialize(string content);
}
