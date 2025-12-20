using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using infraestrutura.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api;

public class Startup
{
    private readonly IConfiguration _config;

    public Startup(IConfiguration configuration)
    {
        _config = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://100.28.1.126:3000",
                        "http://localhost:3000",
                        "https://importtimporta.com.br",
                        "http://importtimporta.com.br"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // JWT
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // Banco
        var cs = _config.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                cs,
                new MySqlServerVersion(new Version(8, 0, 44))
            )
        );

        // AWS Settings
        services.Configure<S3Settings>(_config.GetSection("AWS"));

        // DI Services
        services.AddScoped<IS3Service, S3Service>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IJwtService, JwtService>();

        // Repositories
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
