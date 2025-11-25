using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Auth;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.SignalRHubs;
using FluentValidation;
using RS1_2024_25.API.Endpoints.AuthEndpoints;
using RS1_2024_25.API.Services.Interfaces;
using RS1_2024_25.API.Endpoints.AppUserEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CONFIGURATION
// =====================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// =====================
// DATABASE
// =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db1")));

// =====================
// CHATGPT SERVICE
// =====================
builder.Services.AddScoped<ChatService>();

// =====================
// HTTP CONTEXT ACCESSOR
// =====================
builder.Services.AddHttpContextAccessor();

// =====================
// CUSTOM SERVICES
// =====================
builder.Services.AddTransient<IMyAuthService, MyAuthService>();
builder.Services.AddScoped<IAuthContext, AuthContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSignalR();

// =====================
// CONTROLLERS & SWAGGER
// =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.OperationFilter<MyAuthorizationSwaggerHeader>());

// =====================
// VALIDATION
// =====================
builder.Services.AddValidatorsFromAssemblyContaining<AppUserAddValidator>();

// =====================
// RATE LIMITER
// =====================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("chatLimiter", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;             // max 3 req
        limiterOptions.Window = TimeSpan.FromSeconds(60);
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = 429;
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            response = "Warning: Chatbot is overloaded. Please wait a few seconds before retrying."
        }, ct);
    };
});

// =====================
// JWT AUTHENTICATION
// =====================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Headers.TryGetValue("my-auth-token", out var token))
                    context.Token = token;
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

// =====================
// BUILD APP
// =====================
var app = builder.Build();

// =====================
// MIDDLEWARE PIPELINE
// =====================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options =>
    options.SetIsOriginAllowed(x => true)
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
);

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter(); // activate rate limiter middleware

// =====================
// ENDPOINTS
// =====================
app.MapControllers();
app.MapHub<MySignalrHub>("/mysginalr-hub-path");

app.Run();
