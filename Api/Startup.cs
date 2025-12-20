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

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:3000",
                        "http://100.28.1.126:3000"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // JWT
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
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

        // DB
        var cs = _config.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                cs,
                new MySqlServerVersion(new Version(8, 0, 44))
            )
        );

        // DI
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsEnvironment("Staging"))
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
