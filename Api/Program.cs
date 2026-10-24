using Api;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.AwsRepository.S3;
using Domain.Enuns;
using Domain.Models.UserMenu;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin =>
                    !string.IsNullOrEmpty(origin) &&
                    (origin.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase) ||
                     origin.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase)))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            var corsOrigins = config.GetSection("Cors:Origins").Get<string[]>();
            if (corsOrigins is { Length: > 0 })
            {
                policy.WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
            else
            {
                policy.WithOrigins(
                        "https://importtimporta.com.br",
                        "https://www.importtimporta.com.br",
                        "http://localhost:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        }
    });
});

var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization();

var cs = config.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(cs)
);

builder.Services.Configure<S3Settings>(config.GetSection("S3Settings"));

builder.Services.AddScoped<IS3Repository, S3Repository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IUserService, UsuarioService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
builder.Services.AddScoped<IEnderecoService, EnderecoService>();
builder.Services.AddScoped<IFreteService, FreteService>();

builder.Services.AddHttpClient("Frete", client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IQueueService, QueueService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

            if (!db.Menus.Any())
            {
                db.Menus.AddRange(
                    new UserMenu { Id = Guid.NewGuid(), Name = "Minha conta", Link = "/minha-conta", Icon = "person", Role = Roles.User },
                    new UserMenu { Id = Guid.NewGuid(), Name = "Minha conta", Link = "/minha-conta", Icon = "person", Role = Roles.Admin },
                    new UserMenu { Id = Guid.NewGuid(), Name = "Painel", Link = "/painel-administrador", Icon = "dashboard", Role = Roles.Admin },
                    new UserMenu { Id = Guid.NewGuid(), Name = "Produtos", Link = "/painel-administrador/produtos", Icon = "inventory", Role = Roles.Admin },
                    new UserMenu { Id = Guid.NewGuid(), Name = "Usuários", Link = "/painel-administrador/usuarios", Icon = "group", Role = Roles.Admin }
                );
                db.SaveChanges();
            }

            break;
        }
        catch
        {
            retries--;
            Console.WriteLine($"Banco não disponível. Tentativas restantes: {retries}");
            if (retries == 0) throw;
            Thread.Sleep(delay);
        }
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Importt Importa API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();