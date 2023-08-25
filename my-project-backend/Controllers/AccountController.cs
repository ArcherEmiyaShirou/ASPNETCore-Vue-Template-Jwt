using AutoMapper;
using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Backend.Contract.Entity.VO;
using Backend.Service.Implementation;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Buffers;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Principal;

namespace my_project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [EnableRateLimiting("fixed")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly IPasswordHasher passwordHasher;
        private readonly AccountDbContext dbContext;
        private readonly IAccountService accountService;
        private readonly IMemoryCache memoryCache;

        public AccountController(IConfiguration configuration,
            ILogger<AccountController> logger,
            JwtHelper jwtHelper,
            IPasswordHasher passwordHasher,
            AccountDbContext dbContext,
            IAccountService accountService,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _logger = logger;
            _jwtHelper = jwtHelper;
            this.passwordHasher = passwordHasher;
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.memoryCache = memoryCache;
            dbContext.Database.EnsureCreated();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var context = HttpContext;

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
        public async Task<CustomResponse<object>> Register(EmailRegisterVO vo)
        {
            string res = await accountService.RegistEmailAccountAsync(vo);
            return CustomResponse<object>.Success(res);
        }

        [HttpPost]
        public CustomResponse<object> ResetConfirm(ConfirmResetVO vo)
        {
            return CustomResponse<object>.Success(accountService.ResetConfirm(vo));
        }

        [HttpPost]
        public CustomResponse<object> CheckVerifyCode(string email, string code)
        {
            var service = accountService as AccountService;
            bool res = service!.GetEmailVerifyCode(email) == code;
            return res ? CustomResponse<object>.Success() : CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, "验证码错误！");
        }

        [HttpPost]
        public async Task<IActionResult> LoginTest(string password)
        {
            string target = dbContext.Find<Account>(1L)?.Password ?? throw new KeyNotFoundException(password);
            bool res = await passwordHasher.VerifyPasswordAsync(password, target);
            return res switch
            {
                true => Ok(res),
                false => NotFound()
            };
        }

        [HttpPost]
        public async Task<CustomResponse<object>> ResetPassword(EmailResetVO vo)
        {
            return CustomResponse<object>.Success(await accountService.ResetEmailAccountPasswordAsync(vo));
        }

        // Test Action
        [Authorize]
        [HttpPost]
        public CustomResponse<object> TestLogout()
        {
            string res = accountService.Logout(HttpContext);
            if (res != string.Empty)
                return CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, res);
            return CustomResponse<object>.Success("退出成功！");
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestAuthrize()
        {
            return Ok();
        }
    }
}
