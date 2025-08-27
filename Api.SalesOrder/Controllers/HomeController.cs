using Microsoft.AspNetCore.Mvc;

namespace Api.SalesOrder.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is running...");
        }
    }
}
