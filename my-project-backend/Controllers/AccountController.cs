using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Backend.Service.Implementation;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using System.Net;

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
        private readonly IAccountService accountService;

        public AccountController(IConfiguration configuration,
            ILogger<AccountController> logger,
            JwtHelper jwtHelper,
            IPasswordHasher passwordHasher,
            AccountDbContext dbContext,
            IAccountService accountService)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtHelper = jwtHelper;
            this.passwordHasher = passwordHasher;
            this.dbContext = dbContext;
            this.accountService = accountService;
            dbContext.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(CustomResponse<object>.Success(_jwtHelper.CreateToken()));
        }

        [HttpGet]
        public CustomResponse<object> AskVerifyCode(string email, string type)
        {
            if (Request.HttpContext is { Connection.RemoteIpAddress: not null })
            {
                string res = accountService.RegistEmailVerifyCode(type, email, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                return string.IsNullOrWhiteSpace(res) switch
                {
                    true => CustomResponse<object>.Success(),
                    false => CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, res)
                };
            }

            return CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, "请检查您的网络连接！");

        }

        [HttpPost]
        public CustomResponse<object> CheckVerifyCode(string email,string code)
        {
            var service = accountService as AccountService;
            bool res = service!.GetEmailVerifyCode(email) == code;
            return res ? CustomResponse<object>.Success() : CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, "验证码错误！");
        }

        [HttpPost]
        public async Task<IActionResult> LoginTestAsync(string password)
        {
            string target = dbContext.Find<Account>(1L)?.Password ?? throw new KeyNotFoundException(password);
            bool res = await passwordHasher.VerifyPasswordAsync(password, target);
            return res switch
            {
                true => Ok(res),
                false => NotFound()
            };
        }
    }
}
