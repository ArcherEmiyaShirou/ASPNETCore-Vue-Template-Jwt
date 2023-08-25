using Backend.Contract.Entity;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace my_project_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthorizeService authorizeService;

        public AuthorizeController(IAuthorizeService authorizeService)
        {
            this.authorizeService = authorizeService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<CustomResponse<object>> Login(string username,string password)
        {
            string token = await authorizeService.Login(username, password);

            return CustomResponse<object>.Success(token);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<CustomResponse<object>> Logout()
        {
            await authorizeService.Logout(HttpContext);

            return CustomResponse<object>.Success();
        }
    }
}
