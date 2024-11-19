using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Telegram.Bot;
using vpngenie.API.Authorization;
using vpngenie.API.Handlers;
using vpngenie.API.TelegramBot;
using vpngenie.Application.Services;
using vpngenie.Domain.Interfaces;
using vpngenie.Infrastructure.Data;
using vpngenie.Infrastructure.Repositories;
using XUiLib.Infrastructure;
using XUiLib.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();


services.AddDbContext<ApplicationDbContext>();

services.AddSerilog();
services.AddXUiLib();

var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("auth_cookie"))
                {
                    context.Token = context.Request.Cookies["auth_cookie"];
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_cookie";
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy
        => policy.Requirements.Add(new TelegramUserRequirement(long.Parse(configuration["Telegram:OwnerId"]))));
});

services.AddSingleton<IAuthorizationHandler, TelegramUserHandler>();

services.AddAuthentication();
services.AddAuthorization();

services.AddSwaggerGen();
services.AddControllers();
services.AddCors(options =>
{
    options.AddPolicy("AllowYooKassa", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        policy.WithOrigins("https://api.yookassa.ru")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddSingleton<ITelegramBotClient>(
    new TelegramBotClient(configuration["Telegram:Token"]
                          ?? throw new ArgumentException("Telegram token is null")));


services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPromotionRepository, PromotionRepository>();
services.AddScoped<ITicketRepository, TicketRepository>();
services.AddScoped<IWireGuardRepository, WireGuardRepository>();
services.AddScoped<IServerRepository, ServerRepository>();
services.AddScoped<IPromocodeRepository, PromocodeRepository>();

services.AddScoped<UserService>();
services.AddScoped<PromotionService>();
services.AddScoped<TicketService>();
services.AddScoped<WireGuardService>();
services.AddScoped<ServerService>();
services.AddScoped<PromocodeService>();

services.AddSingleton<BotService>();
services.AddHostedService<BotHostedService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
dbContext.Database.Migrate();

var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowYooKassa");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

logger.LogInformation("vpngenie запущен успешно!");
app.Run();
logger.LogInformation("vpngenie остановлен!");