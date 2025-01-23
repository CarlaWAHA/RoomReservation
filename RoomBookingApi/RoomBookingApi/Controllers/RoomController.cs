using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Data;
using RoomBookingApi.Models;

namespace RoomBookingApi.Controllers
{
    [ApiController]
    [Route("api/{version:apiversion}/{controller}")]
    [ApiVersion("1.0")]
    public class RoomController : ControllerBase
    {
        private readonly RoomApiContext _context;
        private readonly ILogger<RoomController> _logger;

        public RoomController(RoomApiContext context, ILogger<RoomController> logger)
        {
            _context = context;
            _logger=logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            _logger.LogInformation($"get all room");
            return Ok(_context.Rooms);

        }

 [HttpGet]
        [MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<Room>> GetRoomsV2( )
        {
            _logger.LogInformation("Get all room");
            return Ok(_context.Rooms.FirstOrDefault());

        }

        [HttpPost]
        public ActionResult<Room> AddRoom(Room room)
        {
            _logger.LogInformation($"Add room {room}");
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return Created(nameof(AddRoom), room);
        }
    }
}