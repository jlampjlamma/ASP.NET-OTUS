using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PreferenceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PreferenceDb")));

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// 1. Регистрируем Redis как распределённый слой
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Адрес берём из конфигурации (см. appsettings ниже)
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Pcf_Preferences_"; // Префикс ключей, чтобы не смешивать с другими сервисами
});

// 2. HybridCache с правильными опциями
builder.Services.AddHybridCache(options =>
{
    // Глобальные настройки для всех вызовов (можно переопределять локально)
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        // Сколько хранить в ЛОКАЛЬНОМ кэше (память процесса)
        LocalCacheExpiration = TimeSpan.FromSeconds(10),

        // Сколько хранить в РАСПРЕДЕЛЁННОМ кэше (Redis)
        Expiration = TimeSpan.FromSeconds(30),

        // Опционально: лимит на размер одного объекта (по умолчанию 1MB)
        // SizeLimit = 1024 * 1024,

        // Опционально: флаги поведения
        // Flags = HybridCacheEntryFlags.DisableLocalCache // если нужен только Redis
    };

    // Опционально: глобальный лимит на количество записей в локальном кэше
    // (если не задать — используется дефолтное значение, обычно 100-1000)
    // options.MaximumSize = 500;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
initializer.Initialize();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
