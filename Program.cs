using Microsoft.EntityFrameworkCore;
using ParParWebsite.Api.Infrastructure;
using ParParWebsite.Api.Middleware;
using ParParWebsite.Api.Repositories.Interfaces;
using ParParWebsite.Api.Repositories;
using ParParWebsite.Api.Services.Interfaces;
using ParParWebsite.Api.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 2) Register your Repositories
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

// 3) Register your FileService
builder.Services.AddScoped<IFileService, FileService>();

// 4) Register your Application Services (where your business logic lives)
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5174") // Your React dev URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.BufferBody = false;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
