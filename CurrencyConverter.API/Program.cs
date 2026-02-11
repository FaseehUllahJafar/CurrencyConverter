using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Domain.Interfaces;
using CurrencyConverter.Infrastructure.Providers;
using CurrencyConverter.Infrastructure.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Polly;
using Polly.Extensions.Http;
using Asp.Versioning;


var builder = WebApplication.CreateBuilder(args);

//////////////////////////////////////////////////////
// 1️⃣ SERILOG CONFIGURATION (GOES FIRST)
//////////////////////////////////////////////////////

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//////////////////////////////////////////////////////
// 2️⃣ SERVICES
//////////////////////////////////////////////////////

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//////////////////////////////////////////////////////
// 3️⃣ API VERSIONING
//////////////////////////////////////////////////////

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

//////////////////////////////////////////////////////
// 4️⃣ RATE LIMITING
//////////////////////////////////////////////////////

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 60;
        opt.QueueLimit = 2;
    });
});

//////////////////////////////////////////////////////
// 5️⃣ JWT AUTHENTICATION
//////////////////////////////////////////////////////

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});


builder.Services.AddAuthorization();

//////////////////////////////////////////////////////
// 6️⃣ DEPENDENCY INJECTION
//////////////////////////////////////////////////////

builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();

builder.Services.AddHttpClient<ICurrencyProvider, FrankfurterProvider>(client =>
{
    client.BaseAddress = new Uri("https://api.frankfurter.app/");
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retry =>
        TimeSpan.FromSeconds(Math.Pow(2, retry))))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

//////////////////////////////////////////////////////
// 7️⃣ HEALTH CHECKS
//////////////////////////////////////////////////////

builder.Services.AddHealthChecks();

//////////////////////////////////////////////////////
// BUILD APP
//////////////////////////////////////////////////////

var app = builder.Build();

//////////////////////////////////////////////////////
// 8️⃣ MIDDLEWARE PIPELINE
//////////////////////////////////////////////////////

app.UseSerilogRequestLogging();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
