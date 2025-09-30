namespace Hangman.Extensions.DependencyInjection;

/// <summary>Утилиты для удобного извлечения сервисов</summary>
public static class ServiceProviderExtensions
{
    /// <summary>Извлекает обязательный сервис, иначе бросает исключение</summary>
    public static T GetRequiredService<T>(this IServiceProvider sp) where T : class =>
            sp.GetService(typeof(T)) as T ??
            throw new InvalidOperationException($"Service {typeof(T)} is not registered.");
}
