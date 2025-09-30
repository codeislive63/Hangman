using Hangman.Extensions.Configuration;
using Hangman.Extensions.DependencyInjection;
using Hangman.Extensions.Logging;

namespace Hangman.Extensions.Hosting;

/// <summary>Строитель хоста</summary>
public class HostBuilder(string[] args)
{
    public string[] Args { get; private set; } = args;

    public IServiceCollection Services { get; private set; } = new ServiceCollection();

    public ILogger Logger { get; private set; } = new FileLogger();

    public IHangmanConfiguration Configuration { get; private set; } = new HangmanJsonConfiguration();

    /// <summary>Задаёт логгер и регистрирует его в контейнере</summary>
    public HostBuilder UseLogger(ILogger logger)
    {
        Logger = logger;
        Services.AddSingleton(logger);
        return this;
    }

    /// <summary>Грузит зашифрованный конфиг (если файл и ключ доступны) и регистрирует его</summary>
    public HostBuilder LoadConfigurationFromEncrypted(string encPath = "appsettings.enc", string keyEnv = "HANGMAN_KEY")
    {
        Configuration = HangmanJsonConfigurationLoader.LoadSecure(Path.Combine(AppContext.BaseDirectory, encPath), keyEnv, Logger);
        Services.AddSingleton(Configuration);
        return this;
    }

    /// <summary>Добавляет произвольные регистрации сервисов</summary>
    public HostBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(Services);
        return this;
    }

    /// <summary>Собирает Host и создаёт ServiceProvider</summary>
    public Host Build() => new(Args, Services.BuildServiceProvider());
}
