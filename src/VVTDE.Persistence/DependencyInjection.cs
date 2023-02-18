using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VVTDE.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["DbConnection"];
        services.AddDbContext<VideoDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        services.AddSingleton<VideoDbContext>(services => services.GetService<VideoDbContext>());
        return services;
    }
}