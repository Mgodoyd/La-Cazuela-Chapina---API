using System.Text;
using System.Threading.RateLimiting;
using Api.Data;
using Api.Interface;
using Api.Middleware;
using Api.Repositories;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

// Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadena de conexión
var sqlHost = Environment.GetEnvironmentVariable("SQL_HOST");
var sqlPort = Environment.GetEnvironmentVariable("SQL_PORT");
var sqlDatabase = Environment.GetEnvironmentVariable("SQL_DATABASE");
var sqlUser = Environment.GetEnvironmentVariable("SQL_USER");
var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD");

var connectionString =
    $"Server={sqlHost},{sqlPort};Database={sqlDatabase};User Id={sqlUser};Password={sqlPassword};TrustServerCertificate=True;";

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT");

// Conexión a Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = new ConfigurationOptions
    {
        EndPoints = { $"{redisHost}:{redisPort}" },
        AbortOnConnectFail=false,
        Ssl = true
    };
    return ConnectionMultiplexer.Connect(config);
});

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseLazyLoadingProxies();
});

// CORS
var allowedOrigins = new[] { "http://localhost:5173", "http://Albmdwapi-1889324219.us-east-1.elb.amazonaws.com" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 50,
            TokensPerPeriod = 50,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Autenticación y Autorización con JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Sub,

        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

// Inyección de dependencias
// Repositorio genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Repositorios específicos
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IComboRepository, ComboRepository>();

// Servicios de dominio
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<ComboService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SaleService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VoiceCommandService>();
builder.Services.AddScoped<EmbeddingService>();
builder.Services.AddScoped<RedisService>();
// Servicios de voz con HttpClient configurado
builder.Services.AddScoped<SpeechToTextService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("openai");
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new SpeechToTextService(httpClient, configuration);
});

builder.Services.AddScoped<TextToSpeechService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("openai");
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new TextToSpeechService(httpClient, configuration);
});


builder.Services.AddScoped<BusinessContextProvider>();

// Servicios singleton
builder.Services.AddSingleton<JwtService>();
builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
});
builder.Services.AddScoped<OpenAIChatService>();

var app = builder.Build();

// Middlewares personalizados
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseWebSockets();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigins");

// Autenticación antes de Autorización
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// Rutas
app.MapGroup("/api/v1").MapControllers();

app.Run();
