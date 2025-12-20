using Api;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// AWS S3
builder.Services.Configure<S3Settings>(
    builder.Configuration.GetSection("AWS"));

builder.Services.AddSingleton<S3Service>();

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
