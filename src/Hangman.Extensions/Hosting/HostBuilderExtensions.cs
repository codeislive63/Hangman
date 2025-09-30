using Hangman.Extensions.DependencyInjection;
using Hangman.Extensions.Infrastructure;
using System.Reflection;

namespace Hangman.Extensions.Hosting;

/// <summary>Расширения билдера: автопоиск и регистрация контроллеров/представлений из EntryAssembly</summary>
public static class HostBuilderExtensions
{
    /// <summary>Находит и регистрирует все реализации <see cref="IController"/> из входной сборки</summary>
    public static HostBuilder AddControllers(this HostBuilder builder)
        => builder.AddControllersFrom(Assembly.GetEntryAssembly()!);

    /// <summary>Находит и регистрирует все представления (типы, реализующие <see cref="IView"/>)</summary>
    public static HostBuilder AddViews(this HostBuilder builder)
        => builder.AddViewsFrom(Assembly.GetEntryAssembly()!);

    /// <summary>Комбинированная регистрация представлений и контроллеров</summary>
    public static HostBuilder AddControllersWithViews(this HostBuilder builder)
        => builder.AddViews().AddControllers();

    private static HostBuilder AddControllersFrom(this HostBuilder builder, Assembly assembly)
    {
        var controllerContract = typeof(IController);

        Type[] impls = [.. assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && controllerContract.IsAssignableFrom(t))];

        builder.ConfigureServices(services =>
        {
            foreach (var impl in impls)
            {
                services.AddSingleton(typeof(IController), sp => Create(sp, impl));
            }
        });

        return builder;
    }

    private static HostBuilder AddViewsFrom(this HostBuilder builder, Assembly assembly)
    {
        var viewMarker = typeof(IView);

        Type[] impls = [.. assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && viewMarker.IsAssignableFrom(t))];

        builder.ConfigureServices(services =>
        {
            List<Type> directImpls = [.. impls.Where(impl =>
                impl.GetInterfaces().Where(i => i != viewMarker && viewMarker.IsAssignableFrom(i)).Any() == false
            )];

            foreach (var impl in impls)
            {
                Type[] viewIfaces = [.. impl.GetInterfaces().Where(i => i != viewMarker && viewMarker.IsAssignableFrom(i))];

                if (viewIfaces.Length > 0)
                {
                    foreach (var iface in viewIfaces)
                    {
                        services.AddSingleton(iface, sp => Create(sp, impl));
                    }
                }
                else
                {
                    services.AddSingleton(impl, sp => Create(sp, impl));
                }
            }

            // Если нет специализированных интерфейсов и при этом только один класс реализует IView,
            // делаем маппинг IView -> этот единственный класс.
            if (directImpls.Count == 1)
            {
                var only = directImpls[0];
                services.AddSingleton(typeof(IView), sp => Create(sp, only));
            }
            else if (directImpls.Count > 1)
            {
                // Тут неоднозначность: какой из n View отдавать как IView?
                // Либо бросаем исключение, либо выбираем по имени/атрибуту.
                throw new InvalidOperationException(
                    $"Multiple direct IView implementations found ({string.Join(", ", directImpls.Select(t => t.Name))}). " +
                    "Introduce a specialized interface (e.g., IConsoleView : IView) or register one concrete type explicitly.");
            }
        });

        return builder;
    }

    private static object Create(IServiceProvider sp, Type type)
    {
        if (sp is ServiceProvider p)
        {
            return p.CreateInstance(type);
        }

        var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                       .OrderByDescending(c => c.GetParameters().Length)
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"Type {type} has no public constructor.");

        var args = ctor.GetParameters()
                       .Select(param => sp.GetService(param.ParameterType)
                            ?? throw new InvalidOperationException(
                                   $"Unable to resolve {param.ParameterType} for {type}."))
                       .ToArray();

        return Activator.CreateInstance(type, args)!
               ?? throw new InvalidOperationException($"Activator failed to create {type}.");
    }
}
