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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

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
                throw new AddAccountFailureException("内部服务器出错，添加失败！");
            _memoryCache.Remove(Const.VERIFY_EMAIL_LIMIT + email);

            return string.Empty;
        }

        public string RegistEmailVerifyCode(string type, string email, string address)
        {
            string code = (new Random().Next(899999) + 100000).ToString();
            var data = new Dictionary<string, string>
            {
                {"type",Const.EMAIL_TYPE_REGISTRATION },
                {"code", code},
                {"email", email},
            };

            _memoryCache.Set(Const.VERIFY_EMAIL_DATA + email, code, TimeSpan.FromMinutes(3));
            _mediator.Publish(new MediatrData(Const.Event_SendEmail, data));

            return string.Empty;
        }

        public string resetConfirm(ConfirmResetVO info)
        {
            throw new NotImplementedException();
        }

        public string resetEmailAccountPassword(EmailResetVO info)
        {
            throw new NotImplementedException();
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

        public async Task<bool> ExistEmailAccount(string address)
        {
            return await _dbContext.Set<Account>().AnyAsync(ac => ac.Email == address);
        }

        public async Task<bool> ExistUserNameAccount(string userName)
        {
            return await _dbContext.Set<Account>().AllAsync(ac => ac.Username == userName);
        }
    }
}
