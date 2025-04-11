using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data; // adapte si ton namespace est différent

namespace RoomBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet("ping-db")]
        public async Task<IActionResult> PingDatabase([FromServices] ApplicationDbContext context)
        {
            var roomsExist = await context.Rooms.AnyAsync();
            return Ok(new { dbConnected = true, rooms = roomsExist });
        }
    }
}
