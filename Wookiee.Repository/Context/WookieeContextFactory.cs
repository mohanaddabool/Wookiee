using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Wookiee.Repository.Context;

public class WookieeContextFactory: IDesignTimeDbContextFactory<WookieeContext>
{
    public WookieeContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        var builder = new DbContextOptionsBuilder<WookieeContext>();
        var connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

        builder.UseSqlServer(connectionString);

        return new WookieeContext(builder.Options);
    }
}