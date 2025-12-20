using Api;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// aplica migrations com retry simples
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;
    var delay = TimeSpan.FromSeconds(5);

    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Banco não disponível. Tentativas restantes: {retries}");
            if (retries == 0) throw;
            Thread.Sleep(delay);
        }
    }
}

startup.Configure(app, app.Environment);

app.Run();
