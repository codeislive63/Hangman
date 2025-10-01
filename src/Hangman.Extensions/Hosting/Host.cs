using Hangman.Extensions.DependencyInjection;
using Hangman.Extensions.Infrastructure;

namespace Hangman.Extensions.Hosting;

/// <summary>Композиционный корень программы</summary>
public sealed class Host(string[] args, IServiceProvider services)
{
    private readonly string[] _args = args;

    public IServiceProvider Services { get; } = services;

    /// <summary>Запускает первый контроллер, чей <see cref="IController.CanHandle"/> вернёт true.</summary>
    public void Run()
    {
        var controllers = Services.GetRequiredService<IEnumerable<IController>>();
        var controller = controllers.FirstOrDefault(c => c.CanHandle(_args))
                        ?? throw new InvalidOperationException("No suitable controller found.");

        Environment.ExitCode = controller.Run(_args);
    }
}