using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.Data.Seed;
using BuddyErp.Api.Options;
using BuddyErp.Api.Services.Auth;
using BuddyErp.Api.Services.Health;
using BuddyErp.Api.Services.Members;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection 설정이 필요합니다.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

var jwtOptions = builder.Configuration
    .GetRequiredSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "Jwt 설정이 필요합니다.");

if (jwtOptions.SigningKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:SigningKey는 32자 이상이어야 합니다.");
}

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetRequiredSection(
        JwtOptions.SectionName));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "name",
                RoleClaimType = "role",
            };
    });

builder.Services.AddAuthorization();

// Controller가 구현 클래스가 아닌 인터페이스에 의존하도록 연결합니다.
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.Configure<InitialAccountPasswordsOptions>(
    builder.Configuration.GetSection(InitialAccountPasswordsOptions.SectionName));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<InitialAccountSeeder>();

// React 개발 서버에서 오는 요청만 허용합니다.
// CORS는 브라우저가 서로 다른 출처의 API를 호출할 때 필요한 보안 규칙입니다.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (args.Contains("--seed-initial-accounts", StringComparer.Ordinal))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider
        .GetRequiredService<InitialAccountSeeder>();
    var createdAccountCount = await seeder.SeedAsync();

    Console.WriteLine(
        $"초기 고정 계정 생성 완료: {createdAccountCount}개");

    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 로컬 개발은 React와 API를 HTTP로 간단히 연결합니다.
// 배포 환경에서는 HTTP 요청을 반드시 HTTPS로 전환합니다.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendDevelopment");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
