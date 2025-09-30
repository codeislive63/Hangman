namespace Hangman.Extensions.DependencyInjection;

/// <summary>Описание регистрации сервиса в контейнере</summary>
public sealed class ServiceDescriptor
{
    public Type ServiceType { get; }

    public ServiceLifetime Lifetime { get; }

    public Type? ImplementationType { get; }

    public Func<IServiceProvider, object>? ImplementationFactory { get; }

    public object? ImplementationInstance { get; private set; }

    private ServiceDescriptor(
        Type serviceType,
        ServiceLifetime lifetime,
        Type? implementationType,
        Func<IServiceProvider, object>? factory,
        object? instance)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
        ImplementationType = implementationType;
        ImplementationFactory = factory;
        ImplementationInstance = instance;
    }

    // Статические фабрики: Singleton, generic и non-generic...
    public static ServiceDescriptor Singleton<T>(T instance) where T : class =>
        new(typeof(T), ServiceLifetime.Singleton, null, null, instance);

    public static ServiceDescriptor Singleton<TService, TImpl>()
        where TService : class
        where TImpl : class, TService =>
        new(typeof(TService), ServiceLifetime.Singleton, typeof(TImpl), null, null);

    public static ServiceDescriptor Singleton(Type service, Func<IServiceProvider, object> factory) =>
        new(service, ServiceLifetime.Singleton, null, factory, null);

    /// <summary>Присвоить синглтону созданный экземпляр</summary>
    public void SetInstance(object obj) => ImplementationInstance ??= obj;
}