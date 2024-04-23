using Microsoft.AspNetCore.Mvc;
using User.Models;
using User.Interface;

namespace User.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _repo;

        public UserController(ILogger<UserController> logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }
        [HttpGet("/settings")]
        public async Task<ActionResult> GetSettings() {
            try {
                var data = await _repo.GetSettings();

                return Ok(new {
                    code = 200,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("/settings")]
        public async Task<ActionResult> SaveSettings(Settings payload) {
            try {
                await _repo.SaveSettings(payload);

                return Ok(new {
                    code = 200,
                    message = "Settings saved successfuly"
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }

        [HttpPost("/auth")]
        public async Task<ActionResult> Authenticate(AdminDto payload) {
            try {
                var data = await _repo.AdminAuth(payload);

                return Ok(new {
                    code = 200,
                    data
                });
            } catch(Exception e) {
                Console.WriteLine(e.Message);

                var response = new
                {
                    code = 500,
                    status = false,
                    message = "Unnable to complete your request"
                };

                return StatusCode(500, response);
            }
        }
    }
}