using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Auth;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.SignalRHubs;
using FluentValidation;
using FluentValidation.AspNetCore;
using RS1_2024_25.API.Endpoints.AuthEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RS1_2024_25.API.Services.Interfaces;
using RS1_2024_25.API.Endpoints.AppUserEndpoints;



//doing it with builder

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Add services to the container.



builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db1")));






builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.OperationFilter<MyAuthorizationSwaggerHeader>());
builder.Services.AddHttpContextAccessor();

//dodajte vaše servise
builder.Services.AddTransient<IMyAuthService, MyAuthService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IEmailService, EmailService>();

//builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IAuthContext, AuthContext>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IFileService,FileService>();


//Configuring JWT Authentication 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Headers.TryGetValue("my-auth-token", out var token))
                {
                    context.Token = token;
                }
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

//pretrazuje sve validatore iz DLL fajla (tj. projekta) koji sadrži AuthGetEndpoint.css
//builder.Services.AddValidatorsFromAssemblyContaining<AuthGetEndpoint>();//moze se navesti bilo koja klasa iz ovog projekta
builder.Services.AddValidatorsFromAssemblyContaining<AppUserAddValidator>();




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(
    options => options
        .SetIsOriginAllowed(x => _ = true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
); //This needs to set everything allowed

app.UseStaticFiles();




app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MySignalrHub>("/mysginalr-hub-path");

app.Run();
