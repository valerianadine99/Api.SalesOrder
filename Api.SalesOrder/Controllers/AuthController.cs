using Api.SalesOrder.API.Models;
using Api.SalesOrder.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.SalesOrder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        // Mock users for demonstration, this shoould be in tables for managment but thats not the purpose of the test
        private readonly Dictionary<string, (string Password, string Role, string userId)> _users = new()
        {
            { "admin@logistics.com", ("admin123", "Admin", "d900e318-e9ae-427e-8e22-15aa53060f22") },
            { "user@logistics.com", ("vendedor123", "Vendedor", "4471cf7b-42bd-4520-914d-e860a70572dd") }
        };

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (!_users.TryGetValue(model.Username.ToLower(), out var user))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            if (user.Password != model.Password)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(model.Username, user.Role, user.userId);

            return Ok(new
            {
                token,
                username = model.Username,
                role = user.Role,
                expiresIn = 28800 // 8 hours in seconds
            });
        }
    }
}
