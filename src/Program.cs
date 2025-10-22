using Microsoft.EntityFrameworkCore;
using ParParWebsite.Api.Infrastructure;
using ParParWebsite.Api.Middleware;
using ParParWebsite.Api.Services.Interfaces;
using ParParWebsite.Api.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 2) Register your Repositories

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
                .WithOrigins("http://localhost:5173") // Your React dev URL
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

app.Use(async (context, next) =>
{
    // remove double slash issues by normalizing Path
    var path = context.Request.Path.Value ?? "";
    if (path.Contains("/uploads/"))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:5173";
        // good practice if you may vary by origin
        context.Response.Headers["Vary"] = "Origin";
        // optional if CORP bites you
        // context.Response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
    }

    await next();
});

app.UseStaticFiles();

app.MapControllers();

app.Run();
