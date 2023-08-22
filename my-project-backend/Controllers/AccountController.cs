using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace my_project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly IPasswordHasher passwordHasher;
        private readonly AccountDbContext dbContext;

        public AccountController(IConfiguration configuration,ILogger<AccountController> logger,JwtHelper jwtHelper,IPasswordHasher passwordHasher, AccountDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtHelper = jwtHelper;
            this.passwordHasher = passwordHasher;
            this.dbContext = dbContext;

            dbContext.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(CustomResponse<object>.Success(_jwtHelper.CreateToken()));
        }

        [HttpPost]
        public async Task<IActionResult> LoginTestAsync(string password) 
        {
            string target = dbContext.Find<Account>(1L)?.Password ?? throw new KeyNotFoundException(password);
            bool res = await passwordHasher.VerifyPasswordAsync(password,target);
            return res switch
            {
                true => Ok(res),
                false => NotFound()
            };
        }
    }
}
