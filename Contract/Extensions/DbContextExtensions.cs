using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using my_project_backend.Config;

namespace Backend.Contract.Extensions
{
    public static class DbContextExtensions
    {
        public static void AddCustomDbContext <T> (this IServiceCollection services) where T : DbContext
        {
            services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(ConfigurationStringManager.Instance.DbConnectionString);
            });
        }
    }
}
