using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using my_project_backend.Config;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Backend.Common.Utills
{
    public class JwtHelper
    {
        public string CreateToken(string username, string role, string email)
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username), //HttpContext.User.Identity.Name
                new Claim(ClaimTypes.Role, role), //HttpContext.User.IsInRole("r_admin")
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, email),
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationStringManager.Instance.JwtSecretKey));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                ConfigurationStringManager.Instance.JwtIssuer,     //Issuer
                ConfigurationStringManager.Instance.JwtAudience,   //Audience
                claims,                          //Claims,
                null,                    //notBefore
                DateTime.Now.AddDays(7),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public string CreateToken()
        {
            return CreateToken("u_admin", Const.ROLE_ADMIN, "example@host.com");
        }
    }
}
