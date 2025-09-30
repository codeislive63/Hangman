namespace Hangman.Extensions.DependencyInjection;

/// <summary>Удобные методы регистрации сервисов для кастомного DI</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Регистрирует Singleton с готовым экземпляром</summary>
    public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService instance)
            where TService : class
    {
        services.Add(ServiceDescriptor.Singleton(instance));
        return services;
    }

    /// <summary>Регистрирует Singleton с реализацией по типам</summary>
    public static IServiceCollection AddSingleton<TService, TImpl>(this IServiceCollection services)
        where TService : class
        where TImpl : class, TService
    {
        services.Add(ServiceDescriptor.Singleton<TService, TImpl>());
        return services;
    }

    /// <summary>Регистрирует Singleton через фабрику</summary>
    public static IServiceCollection AddSingleton(this IServiceCollection services,
        Type serviceType, Func<IServiceProvider, object> factory)
    {
        services.Add(ServiceDescriptor.Singleton(serviceType, factory));
        return services;
    }

    /// <summary>Создаёт провайдер сервисов</summary>
    public static IServiceProvider BuildServiceProvider(this IServiceCollection services)
        => new ServiceProvider(services);
}
