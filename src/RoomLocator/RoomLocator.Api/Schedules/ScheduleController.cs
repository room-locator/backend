using Microsoft.AspNetCore.Mvc;
using RoomLocator.Business.Schedules.Core;
using RoomLocator.Business.Schedules.Services;

namespace RoomLocator.Schedules;

[ApiController]
[Route("api/schedule")]
public sealed class ScheduleController
{
    private readonly RoomLocatorService _roomLocatorService;

    public ScheduleController(RoomLocatorService roomLocatorService)
    {
        _roomLocatorService = roomLocatorService;
    }

    [HttpGet]
    [Route("rooms/available")]
    public async Task<List<CalculatedRoom>> LocateAvailableRooms([FromQuery] DateTime? desiredTime = null)
    {
        return await _roomLocatorService.LocateAsync(desiredTime);
    }
}
