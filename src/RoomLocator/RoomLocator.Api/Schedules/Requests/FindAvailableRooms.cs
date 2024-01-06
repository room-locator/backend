using System.ComponentModel.DataAnnotations;

namespace RoomLocator.Schedules.Requests;

public sealed class FindAvailableRooms
{
    [Required]
    public DateTime DesiredTime { get; set; }
}
