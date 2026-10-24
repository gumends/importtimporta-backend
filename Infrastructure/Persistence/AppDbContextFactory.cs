using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class AppDbContextFactory 
        : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var apiPath = ResolveApiPath();

            var config = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var cs = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(cs);

            return new AppDbContext(optionsBuilder.Options);
        }

        private static string ResolveApiPath()
        {
            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Api"),
                Path.Combine(Directory.GetCurrentDirectory(), "../Api"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Api")
            };

            foreach (var candidate in candidates)
            {
                var full = Path.GetFullPath(candidate);
                if (File.Exists(Path.Combine(full, "appsettings.json")))
                    return full;
            }

            throw new InvalidOperationException(
                "Não foi possível localizar Api/appsettings.json para criar o DbContext.");
        }
    }
}