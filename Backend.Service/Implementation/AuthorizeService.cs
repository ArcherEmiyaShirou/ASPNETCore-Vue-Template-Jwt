using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Service.Implementation
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IPasswordHasher passwordHasher;
        private readonly JwtHelper jwtHelper;
        private readonly AccountDbContext dbContext;
        private readonly IMemoryCache memoryCache;

        public AuthorizeService(
            IPasswordHasher passwordHasher,
            JwtHelper jwtHelper,
            AccountDbContext dbContext,
            IMemoryCache memoryCache)
        {
            this.passwordHasher = passwordHasher;
            this.jwtHelper = jwtHelper;
            this.dbContext = dbContext;
            this.memoryCache = memoryCache;
        }
        public async Task<AuthorizeVO> Login(string username, string password)
        {
            Account entity = await dbContext.Set<Account>().FirstOrDefaultAsync(ac => ac.Username == username || ac.Email == username) ?? throw new InvalidOperationException("用户名或密码错误！");
            if (!await passwordHasher.VerifyPasswordAsync(password, entity.Password))
                throw new InvalidOperationException("用户名或密码错误！");
            else
            {
                JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                string token = jwtHelper.CreateToken(entity.Username, entity.Role, entity.Email);
                JwtSecurityToken tokenObj = jwtSecurityTokenHandler.ReadJwtToken(token);

                return new AuthorizeVO
                {
                    Username = username,
                    Role = entity.Role,
                    Token = token,
                    Expire = tokenObj.ValidTo
                };
            }
        }

        public async Task Logout(HttpContext httpContext)
        {
            await Task.Run(() =>
            {
                string? jti = httpContext.User.Claims.SingleOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (jti is null)
                    throw new InvalidOperationException("请先登录！");
                string exp = httpContext.User.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Exp).Value;

                var expireTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));

                memoryCache.Set<Boolean>(Const.JWT_BLACK_LIST + jti, true, expireTime);
            });
        }
    }
}
