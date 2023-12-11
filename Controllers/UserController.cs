using Microsoft.AspNetCore.Mvc;
using User.Models;

namespace User.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task Login(UserDto payload) {
            try {

            } catch(Exception e) {
                
            }
        }
        
    }
}