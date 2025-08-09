using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlHost = Environment.GetEnvironmentVariable("SQL_HOST");
var sqlPort = Environment.GetEnvironmentVariable("SQL_PORT");
var sqlDatabase = Environment.GetEnvironmentVariable("SQL_DATABASE");
var sqlUser = Environment.GetEnvironmentVariable("SQL_USER");
var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD");

var connectionString = $"Server={sqlHost},{sqlPort};Database={sqlDatabase};User Id={sqlUser};Password={sqlPassword};TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseLazyLoadingProxies();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapGroup("/api/v1").MapControllers();

app.Run();
