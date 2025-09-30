using FluentAssertions;
using Hangman.Controllers;
using Hangman.Extensions.Logging;

namespace Hangman.Tests;

public class NonInteractiveControllerTests
{
    private sealed class NullLogger : ILogger
    {
        public void LogInformation(string m) { }
        public void LogWarning(string m) { }
        public void LogError(string m, Exception? ex = null) { }
        public void LogDebug(string message) { }
        public void LogCritical(string message, Exception? ex = null) { }
    }

    /// <summary>Выводит маску и результат POS/NEG при валидных аргументах</summary>
    [Fact]
    public void ValidArgs_Prints_Mask_And_POS_NEG()
    {
        var c = new NonInteractiveController(new NullLogger());

        var old = Console.Out;
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            var code = c.Run(["volokno", "tolokno"]);
            code.Should().Be(0);

            var firstLine = sw.ToString()
                              .Split(["\r\n", "\n"], StringSplitOptions.None)
                              .FirstOrDefault()?.Trim() ?? string.Empty;

            firstLine.Should().MatchRegex(@"^\*olokno;(POS|NEG)$");
        }
        finally
        {
            Console.SetOut(old);
        }
    }

    /// <summary>Возвращает код 1 и ;NEG при невалидных аргументах</summary>
    [Fact]
    public void InvalidArgs_Returns1_And_SAFE_Default()
    {
        var c = new NonInteractiveController(new NullLogger());
        using var sw = new StringWriter();
        Console.SetOut(sw);

        var code = c.Run(["a", "bbb"]);

        code.Should().Be(1);
        sw.ToString().Trim().Should().Be(";NEG");
    }
}