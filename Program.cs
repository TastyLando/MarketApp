using MongoDB.Driver;
using Market.Services;
using Market.Config;
using Microsoft.Extensions.Options;
using Market.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);

// MongoDB ayarlarını yapılandırma
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

// MongoDB Client singleton olarak kaydediliyor
builder.Services.AddSingleton<IMongoClient>(sp => 
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.ConnectionString));
    clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(settings.ConnectionTimeout);
    clientSettings.MaxConnectionPoolSize = settings.MaxConnectionPoolSize;
    return new MongoClient(clientSettings);
});

// Servisler scoped olarak kaydediliyor
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();
// ... diğer konfigürasyonlar

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// İlk admin kullanıcısını oluştur
using (var scope = app.Services.CreateScope())
{
    try 
    {
        var adminService = scope.ServiceProvider.GetRequiredService<AdminService>();
        // MongoDB bağlantısını kontrol et
        if (await adminService.IsMongoDbConnected())
        {
            await adminService.CreateInitialAdmin();
            Console.WriteLine("MongoDB bağlantısı başarılı ve admin kullanıcısı kontrol edildi.");
        }
        else
        {
            Console.WriteLine("MongoDB bağlantısı başarısız!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Başlangıç hatası: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
