using Hangman.Application.Abstractions;
using Hangman.Application.Services;
using Hangman.Controllers;
using Hangman.Extensions.DependencyInjection;
using Hangman.Extensions.Hosting;
using Hangman.Extensions.Infrastructure;
using Hangman.Extensions.Logging;
using Hangman.Infrastructure.Repositories;

var builder = HostFactory.CreateBuilder(args);
builder.UseLogger(new FileLogger(Path.Combine(AppContext.BaseDirectory, "logs.txt")));

// ===== НЕИНТЕРАКТИВНЫЙ РЕЖИМ =====
if (args.Length == 2)
{
    builder.ConfigureServices(services =>
    {
        services.AddSingleton<IController, NonInteractiveController>();
    });

    var app = builder.Build();
    app.Run();
    return;
}

// ===== ИНТЕРАКТИВНЫЙ РЕЖИМ =====
builder.LoadConfigurationFromEncrypted();

builder.ConfigureServices(services =>
{
    services.AddSingleton<IWordRepository, WordRepository>();
    services.AddSingleton<IGameSetupService, GameSetupService>();
});

// views + оба контроллера; Host выберет нужный контроллер по CanHandle
builder.AddControllersWithViews();

var interactiveApp = builder.Build();
interactiveApp.Run();
