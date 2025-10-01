using System.Text;

namespace Hangman.Extensions.Logging;

/// <summary>Файловый логгер: пишет строки в указанный путь</summary>
public sealed class FileLogger : ILogger
{
    private readonly object _lock = new();
    private readonly string? _filePath;

    public FileLogger(string? filePath = null)
    {
        _filePath = filePath;

        if (_filePath != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_filePath))!);
        }
    }

    public void LogDebug(string message) => WriteFile("DEBUG", message);

    public void LogInformation(string message) => WriteFile("INFO", message);

    public void LogWarning(string message) => WriteFile("WARNING", message);

    public void LogError(string message, Exception? ex = null) =>
        WriteFile("ERROR", message + (ex == null ? "" : Environment.NewLine + ex));

    public void LogCritical(string message, Exception? ex = null) =>
        WriteFile("CRITICAL", message + (ex == null ? "" : Environment.NewLine + ex));

    private void WriteFile(string level, string message)
    {
        var line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";

        lock (_lock)
        {
            if (_filePath is not null)
            {
                File.AppendAllText(_filePath, line + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}
