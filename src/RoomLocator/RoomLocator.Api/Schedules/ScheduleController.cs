using Microsoft.AspNetCore.Mvc;
using RoomLocator.Business.Schedules;

namespace RoomLocator.Schedules;

[ApiController]
[Route("api/schedule")]
public sealed class ScheduleController
{
    private readonly ScheduleService _scheduleService;

    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [Route("available-rooms")]
    public async Task<List<RichRoom>> FindAvailableRooms([FromQuery] DateTime? desiredTime = null)
    {
        return await _scheduleService.FindAvailableRoomsAsync(desiredTime);
    }
}
