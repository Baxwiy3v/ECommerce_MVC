using Malefashion.DAL;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Malefashion
{
    public static class DI
    {
        public static IServiceCollection AddDbConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(configuration.GetConnectionString("Default")));
            

            return services;
        }
    }
}
