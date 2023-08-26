using Backend.Contract.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Service.Interface
{
    public interface IAuthorizeService
    {
        Task<AuthorizeVO> Login(string username, string password);
        Task Logout(HttpContext httpContext);
    }
}
