using BuddyErp.Api.Data;
using BuddyErp.Api.Data.Entities;
using BuddyErp.Api.Data.Seed;
using BuddyErp.Api.Options;
using BuddyErp.Api.Services.Auth;
using BuddyErp.Api.Services.Attendance;
using BuddyErp.Api.Services.Health;
using BuddyErp.Api.Services.Formations;
using BuddyErp.Api.Services.Members;
using BuddyErp.Api.Services.Inventory;
using BuddyErp.Api.Services.Schedules;
using BuddyErp.Api.Services.Expenses;
using BuddyErp.Api.Services.Announcements;
using BuddyErp.Api.Services.Dues;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
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
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IFormationService, FormationService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IMemberDueService, MemberDueService>();

builder.Services.Configure<InitialAccountPasswordsOptions>(
    builder.Configuration.GetSection(InitialAccountPasswordsOptions.SectionName));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<InitialAccountSeeder>();

var allowedFrontendOrigins = new[] { "http://localhost:5173" }
    .Concat(
        builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [])
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

// 로컬 React 개발 서버와 환경변수로 등록한 배포 프론트 주소만 허용합니다.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedFrontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    // Cloudtype 프록시 주소는 배포마다 달라질 수 있습니다.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 새 배포 DB가 비어 있어도 서버 시작 시 EF Core가 필요한 테이블을 만듭니다.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

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

app.UseForwardedHeaders();

// 로컬 개발은 React와 API를 HTTP로 간단히 연결합니다.
// 배포 환경에서는 HTTP 요청을 반드시 HTTPS로 전환합니다.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
