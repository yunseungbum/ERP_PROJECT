var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
