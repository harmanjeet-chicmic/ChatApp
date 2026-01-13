using ChatApp.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatApp.API.Middlewares;
using ChatApp.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChatApp.API.SignalR;
using ChatApp.Infrastructure.Services;
using ChatApp.Application.Common;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Infrastructure.Auth;
using System.Transactions;
using System.Collections.Specialized;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Add services to the container
// ----------------------------------------------------

// Controllers
builder.Services.AddControllers();

// Swagger (for development/testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection (Infrastructure + Application)
builder.Services.AddInfrastructure(builder.Configuration);


var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        )
    };

    // 🔥 THIS IS CRITICAL FOR SIGNALR 🔥
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});


//----using singalR

builder.Services.AddSignalR();

builder.Services.AddSingleton<UserPresenceTracker>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// builder.Services.Configure<EmailSettings>(
//     builder.Configuration.GetSection("EmailSettings"));

 builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<GoogleTokenValidator>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();


// ----------------------------------------------------
// Build app
// ----------------------------------------------------
var app = builder.Build();

// ----------------------------------------------------
// Configure the HTTP request pipeline
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication & Authorization
// (JWT middleware will be plugged in later)

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("ClientCorsPolicy"); 

app.UseAuthentication();  
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

// ----------------------------------------------------
// Run application
// ----------------------------------------------------
app.Run();
