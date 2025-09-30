using FluentAssertions;
using Hangman.Extensions.Logging;

namespace Hangman.Extensions.Tests;

public class FileLoggerTests
{
    /// <summary>Логгер пишет сообщения в файл</summary>
    [Fact]
    public void Writes_Messages_To_File()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".log");
        try
        {
            var log = new FileLogger(path);
            log.LogInformation("info");
            log.LogDebug("debug");
            log.LogWarning("warn");
            log.LogError("err", new InvalidOperationException("boom"));
            log.LogCritical("critical", new InvalidOperationException("kaboom"));

            var text = File.ReadAllText(path);
            text.Should().Contain("info")
                .And.Contain("debug")
                .And.Contain("warn")
                .And.Contain("err")
                .And.Contain("critical");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
