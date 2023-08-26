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
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class AccountController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IAccountService accountService;

        public AccountController(JwtHelper jwtHelper,
            AccountDbContext dbContext,
            IAccountService accountService)
        {
            _jwtHelper = jwtHelper;
            this.accountService = accountService;
            dbContext.Database.EnsureCreated();
        }

        
    }
}
