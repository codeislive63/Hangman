using System.Reflection;

namespace Hangman.Extensions.DependencyInjection;

/// <summary>Простой DI-контейнер, поддерживает множественные регистрации</summary>
public class ServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, List<ServiceDescriptor>> _map;

    public ServiceProvider(IEnumerable<ServiceDescriptor> descriptors)
    {
        _map = [];

        foreach (var d in descriptors)
        {
            if (!_map.TryGetValue(d.ServiceType, out var list))
            {
                list = [];
                _map[d.ServiceType] = list;
            }

            list.Add(d);
        }
    }

    /// <summary>Ищет и возвращает сервис по типу</summary>
    public object? GetService(Type serviceType)
    {
        if (serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var itemType = serviceType.GetGenericArguments()[0];
            var items = ResolveAll(itemType);
            var array = Array.CreateInstance(itemType, items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                array.SetValue(items[i], i);
            }

            return array;
        }

        if (_map.TryGetValue(serviceType, out var list) && list.Count > 0)
        {
            var d = list[^1];
            if (d.Lifetime == ServiceLifetime.Singleton)
            {
                if (d.ImplementationInstance is not null)
                {
                    return d.ImplementationInstance;
                }

                var obj = CreateFromDescriptor(d);
                d.SetInstance(obj);
                return obj;
            }

            return CreateFromDescriptor(d);
        }

        return !serviceType.IsAbstract && !serviceType.IsInterface ? CreateInstance(serviceType) : null;
    }

    private List<object> ResolveAll(Type serviceType)
    {
        var result = new List<object>();

        if (_map.TryGetValue(serviceType, out var list))
        {
            foreach (var d in list)
            {
                if (d.Lifetime == ServiceLifetime.Singleton)
                {
                    if (d.ImplementationInstance is null)
                    {
                        d.SetInstance(CreateFromDescriptor(d));
                    }

                    result.Add(d.ImplementationInstance!);
                }
                else
                {
                    result.Add(CreateFromDescriptor(d));
                }
            }
        }

        return result;
    }

    private object CreateFromDescriptor(ServiceDescriptor d)
    {
        if (d.ImplementationInstance is not null)
        {
            return d.ImplementationInstance;
        }

        if (d.ImplementationFactory is not null)
        {
            return d.ImplementationFactory(this);
        }

        var typeToCreate = d.ImplementationType ?? d.ServiceType;
        return CreateInstance(typeToCreate);
    }

    /// <summary>Создаёт экземпляр типа, разрешая зависимости через контейнер</summary>
    public object CreateInstance(Type type)
    {
        var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"Type {type} has no public constructor.");

        var args = ctor.GetParameters()
                       .Select(p => GetService(p.ParameterType)
                                ?? throw new InvalidOperationException(
                                       $"Unable to resolve {p.ParameterType} for {type}."))
                       .ToArray();

        return Activator.CreateInstance(type, args)!
               ?? throw new InvalidOperationException($"Activator failed to create {type}.");
    }
}
