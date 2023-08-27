using System.Text;
using System.Threading.RateLimiting;
using Backend.Common.Utills;
using Backend.Common.Utills.Contract;
using Backend.Contract.Dal;
using Backend.Contract.Extensions;
using Backend.Service.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using my_project_backend.Config;
using my_project_backend.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInitializer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    // �����Ҫ�����֤����������������
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             Array.Empty<string>()
         }
     });
});
// add services
builder.Services.AddProjectServices();
builder.Services.AddSingleton(typeof(JwtHelper));
// logger
builder.Services.AddSerilog(config =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});
// Rate Limit
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 4,
                Window = TimeSpan.FromSeconds(12)
            }));
    options.AddPolicy("email_code",httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1,
                Window = TimeSpan.FromMinutes(3)
            }));
});
//MemoryCache
builder.Services.AddMemoryCache();
// PasswordHasher
builder.Services.AddSingleton<IPasswordHasher, SHA256PasswordHasher>();
//DbContext
builder.Services.AddCustomDbContext<AccountDbContext>();
//AutoMapper
builder.Services.AddMyAutoMapper();
// Authentication 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, //�Ƿ���֤Issuer
            ValidIssuer = ConfigurationStringManager.Instance.JwtIssuer, //������Issuer
            ValidateAudience = true, //�Ƿ���֤Audience
            ValidAudience = ConfigurationStringManager.Instance.JwtAudience, //������Audience
            ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationStringManager.Instance.JwtSecretKey)), //SecurityKey
            ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
            ClockSkew = TimeSpan.FromMinutes(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
            RequireExpirationTime = true,
        };
    });
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomResponseBodyMiddlewareResultHandler>();
//Authorization
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
//Mediatr
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", config =>
    {
        config.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseRateLimiter();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("cors");

app.UseAuthentication();

app.UseMiddleware<CheckJwtBlackListMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
