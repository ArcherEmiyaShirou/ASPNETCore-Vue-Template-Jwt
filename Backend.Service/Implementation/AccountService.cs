using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Common.Exception;
using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Backend.Contract.Entity.DTO;
using Backend.Contract.Entity.VO;
using Backend.Service.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Backend.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly AccountDbContext _dbContext;
        private readonly ILogger<AccountService> _logger;
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly IPasswordHasher _passwordHasher;

        public AccountService(AccountDbContext dbContext, 
            ILogger<AccountService> logger, 
            IMediator mediator, 
            IMemoryCache memoryCache,
            IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mediator = mediator;
            _memoryCache = memoryCache;
            _passwordHasher = passwordHasher;
        }

        public async Task<Account?> FindAccountByNameOrEmail(string emailOrName)
        {
            return await _dbContext.Set<Account>().SingleOrDefaultAsync(ac => ac.Email == emailOrName || ac.Username == emailOrName);
        }

        public async Task<string> RegistEmailAccountAsync(EmailRegisterVO info)
        {
            string email = info.Email ?? throw new InvalidOperationException("邮箱地址是空的！");
            string code = info.Code ?? throw new InvalidOperationException("请输入验证码！");
            string password = info.Password ?? throw new InvalidOperationException("密码不符合要求！");
            string cache_code = GetEmailVerifyCode(email) ?? throw new InvalidOperationException("请先获取验证码！");
            string userName = info.Username ?? throw new InvalidOperationException("用户名非法！");

            if (cache_code != code) 
                throw new InvalidOperationException("验证码错误！");
            if (await ExistEmailAccount(email)) 
                throw new InvalidOperationException("该邮件地址已被注册!");
            if (await ExistUserNameAccount(userName)) 
                throw new InvalidOperationException("用户名已存在，请重新更换！");
            string encodedPassword = await _passwordHasher.HashPasswordAsync(password);

            Account account = new Account
            {
                Email = email,
                Password = encodedPassword,
                Username = userName,
                Role = Const.ROLE_USER
            };

            await _dbContext.Set<Account>().AddAsync(account);
            int effectedRow = _dbContext.SaveChanges();

            if (effectedRow == 0)
                throw new AddAccountFailureException("内部服务器出错，请联系管理员！！");
            else 
                DeleteEmailVerifyCode(email);

            return string.Empty;
        }

        public string RegistEmailVerifyCode(string type, string email, string address)
        {
            string code = (new Random().Next(899999) + 100000).ToString();
            var data = new Dictionary<string, string>
            {
                {"type", type },
                {"code", code},
                {"email", email},
            };

            _memoryCache.Set(Const.VERIFY_EMAIL_DATA + email, code, TimeSpan.FromMinutes(3));
            _mediator.Publish(new MediatrData(Const.Event_SendEmail, data));

            return string.Empty;
        }

        public string ResetConfirm(ConfirmResetVO info)
        {
            string email = info.Email ??
                throw new InvalidOperationException("邮箱不能为空！");
            string code = info.Code ?? 
                throw new InvalidOperationException("请输入验证码！！");
            string resetCode = GetEmailVerifyCode(email) ?? 
                throw new InvalidOperationException("请先获取验证码！");

            if (code != resetCode) throw new InvalidOperationException("验证码输入错误，请重新输入！");

            return string.Empty;
        }

        public void DeleteEmailVerifyCode(string email)
        {
            _memoryCache.Remove(Const.VERIFY_EMAIL_DATA + email);
        }

        public async Task<string> ResetEmailAccountPasswordAsync(EmailResetVO info)
        {
            _ = ResetConfirm(new() { Code = info.code, Email = info.email });
            string email = info.email!;
            string rawPassword = info.password ?? throw new InvalidOperationException("请输入密码！");

            Account oldAccount = await FindAccountByNameOrEmail(email) ?? throw new InvalidOperationException("未找到对应的账户！");
            oldAccount.Password = await _passwordHasher.HashPasswordAsync(rawPassword);
            int affectedRows = await _dbContext.SaveChangesAsync();
            if (affectedRows == 0)
                throw new InvalidOperationException("保存失败，请联系管理员！");

            return string.Empty;
        }

        public string Logout(HttpContext httpContext)
        {


            return string.Empty;
        }

        public bool VerifyLimit(string address)
        {
            string key = Const.VERIFY_EMAIL_LIMIT + address;
            return _memoryCache.TryGetValue<string>(key,out _);
        }

        public string? GetEmailVerifyCode(string email)
        {
            return _memoryCache.Get<string>(Const.VERIFY_EMAIL_DATA + email);
        }

        public async Task<bool> ExistEmailAccount(string email)
        {
            return await _dbContext.Set<Account>().AnyAsync(ac => ac.Email == email);
        }

        public async Task<bool> ExistUserNameAccount(string userName)
        {
            return await _dbContext.Set<Account>().AnyAsync(ac => ac.Username == userName);
        }
    }
}
