using ApiWithServiceBus.Models;
using ApiWithServiceBus.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithServiceBus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAzureServiceBus _azureServiceBus;

        public UserController(IAzureServiceBus azureServiceBus)
        {
            _azureServiceBus = azureServiceBus;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {
            await _azureServiceBus.SendMessage(user);
            return Ok("User registered and message sent to Service Bus.");
        }
    }
}
