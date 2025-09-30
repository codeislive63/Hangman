namespace Hangman.Extensions.Hosting;

/// <summary>Фабрика для удобного создания <see cref="HostBuilder"/></summary>
public static class HostFactory
{
    /// <summary>Создаёт новый <see cref="HostBuilder"/> с переданными аргументами</summary>
    public static HostBuilder CreateBuilder(string[] args) => new(args);
}
