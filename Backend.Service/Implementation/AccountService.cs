using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Contract.Dal;
using Backend.Contract.Entity;
using Backend.Service.Interface;
using Microsoft.Extensions.Logging;

namespace Backend.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly AccountDbContext _dbContext;
        private readonly ILogger<AccountService> _logger;

        public AccountService(AccountDbContext dbContext, ILogger<AccountService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public AuthorizeVO Login()
        {
            throw new NotImplementedException();
        }
    }
}
