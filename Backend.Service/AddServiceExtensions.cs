using Backend.Service.Implementation;
using Backend.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Service.Extensions
{
    public static class AddServiceExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailSerivce,EmailService>();
            services.AddScoped<IAccountService,AccountService>();

            return services;
        }
    }
}
