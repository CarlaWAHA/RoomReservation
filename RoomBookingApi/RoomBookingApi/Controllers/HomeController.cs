using Microsoft.AspNetCore.Mvc;

namespace RoomBookingApi.Controllers
{
    [ApiController]
    [Route("api/Home")]
    public class HomeController : ControllerBase
    {
        [HttpGet()]
        public string GetHome()
        {
            return "Hello World !";
        }
    }
}