using Backend.Contract.Entity;
using Microsoft.AspNetCore.Mvc;
using my_project_backend.Utills;

namespace my_project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly JwtHelper _jwtHelper;

        public AccountController(IConfiguration configuration,ILogger<AccountController> logger,JwtHelper jwtHelper)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(CustomResponse<object>.Success(_jwtHelper.CreateToken()));
        }
    }
}
