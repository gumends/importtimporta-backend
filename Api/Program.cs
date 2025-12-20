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

    var tentativas = 10;
    while (tentativas > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            tentativas--;
            Console.WriteLine($"Banco não disponível. Tentativas restantes: {tentativas}");
            Thread.Sleep(3000);
        }
    }
}

startup.Configure(app, app.Environment);

app.Run();
