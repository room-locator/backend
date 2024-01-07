using System.ComponentModel.DataAnnotations;

namespace RoomLocator.Schedules.Requests;

public sealed class FindAvailableRooms
{
    public DateTime DesiredTime { get; set; }
}
