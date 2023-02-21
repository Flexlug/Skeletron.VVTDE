using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VVTDE.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {   
        var connectionString = configuration["DbConnection"];
        var dbFolderName = configuration["DbFolder"];

        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = Path.GetFullPath(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                dbFolderName,
                builder.DataSource));
        connectionString = builder.ToString();

        services.AddDbContextFactory<VideoDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        // Must be singleton
        // You can't depend on object with shorter lifetime than host object
        services.AddSingleton<IVideoDbContext>(services =>
        {
            var contextFactory = services.GetRequiredService<IDbContextFactory<VideoDbContext>>();
            return contextFactory.CreateDbContext();
        });
        return services;
    }
}