using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Contract.Extensions
{
    public static class ScanAutoMapperProfile
    {
        public static IServiceCollection AddMyAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddMaps(System.AppDomain.CurrentDomain.GetAssemblies());
            });

            return services;
        }
    }
}
