using System.Text;
using Backend.Contract.Dal;
using Backend.Contract.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using my_project_backend.Config;
using my_project_backend.Utills;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, CustomResponseBodyMiddlewareResultHandler>();
builder.Services.AddSingleton(typeof(JwtHelper));

// logger
builder.Services.AddSerilog(config =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Hour);
});

//DbContext
builder.Services.AddCustomDbContext<AccountDbContext>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true, //�Ƿ���֤Issuer
            ValidIssuer = Environment.GetEnvironmentVariable("Jwt:Issuer"), //������Issuer
            ValidateAudience = true, //�Ƿ���֤Audience
            ValidAudience = Environment.GetEnvironmentVariable("Jwt:Audience"), //������Audience
            ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt:SecretKey") ?? throw new InvalidOperationException())), //SecurityKey
            ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
            ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
            RequireExpirationTime = true,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
