using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace dm.DYT.Data
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
