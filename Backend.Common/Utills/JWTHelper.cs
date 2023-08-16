using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Backend.Common.Utills
{
    public class JwtHelper
    {
        public string CreateToken()
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "u_admin"), //HttpContext.User.Identity.Name
                new Claim(ClaimTypes.Role, "r_admin"), //HttpContext.User.IsInRole("r_admin")
                new Claim(JwtRegisteredClaimNames.Jti, "admin"),
                new Claim("Username", "Admin"),
                new Claim("Name", "超级管理员")
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt:SecretKey") ?? throw new InvalidOperationException()));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("Jwt:Issuer"),     //Issuer
                Environment.GetEnvironmentVariable("Jwt:Audience"),   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddSeconds(30),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
