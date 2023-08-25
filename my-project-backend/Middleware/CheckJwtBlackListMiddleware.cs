using Backend.Common.Utills;
using Backend.Contract.Entity;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace my_project_backend.Middleware
{
    public class CheckJwtBlackListMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IMemoryCache memoryCache;

        public CheckJwtBlackListMiddleware(RequestDelegate next,IMemoryCache memoryCache)
        {
            this.next = next;
            this.memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(IsTokenBlackListed(context))
            {
                await context.Response.WriteAsJsonAsync<CustomResponse<object>>(CustomResponse<object>.UnAuthorized("您已退出登录！"));
                return;
            }
            await next.Invoke(context);
        }

        private bool IsTokenBlackListed(HttpContext context)
        {
            Claim? claim = context.User.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti);
            if (claim == null)
                return false;
            return memoryCache.TryGetValue(Const.JWT_BLACK_LIST + claim.Value, out _);
        }
    }
}
