namespace Hangman.Extensions.Logging;

/// <summary>Минимальный интерфейс логгера</summary>
public interface ILogger
{
    void LogDebug(string message);

    void LogInformation(string message);

    void LogWarning(string message);

    void LogError(string message, Exception? ex = null);

    void LogCritical(string message, Exception? ex = null);
}
