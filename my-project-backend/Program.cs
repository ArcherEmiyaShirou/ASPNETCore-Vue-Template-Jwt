using System.Text;
using Backend.Common.Utills;
using Backend.Contract.Dal;
using Backend.Contract.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using my_project_backend.Config;
using my_project_backend.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, CustomResponseBodyMiddlewareResultHandler>();
builder.Services.AddSingleton(typeof(JwtHelper));
// logger
builder.Services.AddSerilog(config =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

//DbContext
builder.Services.AddCustomDbContext<AccountDbContext>();
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
//Authorization
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
