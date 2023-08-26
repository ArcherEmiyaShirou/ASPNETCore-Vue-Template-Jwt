using Backend.Contract.Entity;
using Backend.Contract.Entity.VO;
using Backend.Service.Implementation;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace my_project_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableCors("cors")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthorizeService authorizeService;
        private readonly IAccountService accountService;

        public AuthorizeController(IAuthorizeService authorizeService,IAccountService accountService)
        {
            this.authorizeService = authorizeService;
            this.accountService = accountService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<CustomResponse<AuthorizeVO>> Login([FromForm] string username, [FromForm] string password)
        {
            AuthorizeVO response = await authorizeService.Login(username, password);

            return CustomResponse<AuthorizeVO>.Success(response);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<CustomResponse<object>> Logout()
        {
            await authorizeService.Logout(HttpContext);

            return CustomResponse<object>.Success();
        }

        [HttpGet("ask-code")]
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

        [HttpPost("register")]
        public async Task<CustomResponse<object>> Register([FromBody] EmailRegisterVO vo)
        {
            string res = await accountService.RegistEmailAccountAsync(vo);
            return CustomResponse<object>.Success(res);
        }

        [HttpPost("reset-confirm")]
        public CustomResponse<object> ResetConfirm([FromBody] ConfirmResetVO vo)
        {
            return CustomResponse<object>.Success(accountService.ResetConfirm(vo));
        }

        [HttpPost("reset-password")]
        public async Task<CustomResponse<object>> ResetPassword([FromBody] EmailResetVO vo)
        {
            return CustomResponse<object>.Success(await accountService.ResetEmailAccountPasswordAsync(vo));
        }

        [HttpGet]
        [Authorize]
        public IActionResult TestAuthrize()
        {
            return Ok();
        }
    }
}
