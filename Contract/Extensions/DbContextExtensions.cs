using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Contract.Extensions
{
    public static class DbContextExtensions
    {
        public static void AddCustomDbContext <T> (this IServiceCollection services) where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultDB:ConnStr") ??
                                     throw new InvalidOperationException("Connection string is null,plz check the connection string in System Environment !"));
            });
        }
    }
}
