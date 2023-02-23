using System.Runtime.InteropServices.JavaScript;
using VVTDE.Persistence;
using VVTDE.Services;
using VVTDE.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // добавляем сервисы MVC
builder.Services.AddOptions();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSingleton<IVideoStorageService, VideoStorageService>();
builder.Services.AddSingleton<IVideoDownloadService, VideoDownloadService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    var context = serviceProvider.GetRequiredService<VideoDbContext>();
    DbInitializer.Initialize(context);

    logger.LogInformation("Db initialized");
}

// устанавливаем сопоставление маршрутов с контроллерами
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Video}/{action=Watch}/{id?}");

app.Run();