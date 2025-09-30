using FluentAssertions;
using Hangman.Extensions.DependencyInjection;
using Hangman.Extensions.Hosting;
using Hangman.Extensions.Infrastructure;
using Hangman.Extensions.Logging;

namespace Hangman.Extensions.Tests;
public class HostControllerSelectionTests
{
    private sealed class DummyLogger : ILogger
    {
        public void LogInformation(string m) { }
        public void LogWarning(string m) { }
        public void LogError(string m, Exception? ex = null) { }
        public void LogDebug(string message) { }
        public void LogCritical(string message, Exception? ex = null) { }
    }

    private sealed class InteractiveFake : IController
    {
        public bool CanHandle(string[] args) => args.Length != 2;
        public int Run(string[] args) { Environment.ExitCode = 111; return 111; }
    }

    private sealed class NonInteractiveFake : IController
    {
        public bool CanHandle(string[] args) => args.Length == 2;
        public int Run(string[] args) { Environment.ExitCode = 222; return 222; }
    }

    private static readonly string[] args = ["a", "b"];

    /// <summary>Выбирает неинтерактивный контроллер при двух аргументах</summary>
    [Fact]
    public void Picks_NonInteractive_When_Args2()
    {
        var host = new HostBuilder(args)
            .UseLogger(new DummyLogger())
            .ConfigureServices(s =>
            {
                s.AddSingleton(typeof(IController), _ => new InteractiveFake());
                s.AddSingleton(typeof(IController), _ => new NonInteractiveFake());
            })
            .Build();

        host.Run();
        Environment.ExitCode.Should().Be(222);
    }

    /// <summary>Выбирает интерактивный контроллер когда аргументов не два</summary>
    [Fact]
    public void Picks_Interactive_When_Not2()
    {
        var host = new HostBuilder([])
            .UseLogger(new DummyLogger())
            .ConfigureServices(s =>
            {
                s.AddSingleton(typeof(IController), _ => new InteractiveFake());
                s.AddSingleton(typeof(IController), _ => new NonInteractiveFake());
            })
            .Build();

        host.Run();
        Environment.ExitCode.Should().Be(111);
    }
}