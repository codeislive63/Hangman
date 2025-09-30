namespace Hangman.Extensions.Infrastructure;

/// <summary>Единый интерфейс для всех контроллеров (режимов запуска приложения)</summary>
public interface IController
{
    /// <summary>
    /// Проверяет, может ли данный контроллер обработать указанные аргументы командной строки
    /// </summary>
    bool CanHandle(string[] args);

    /// <summary>Выполняет основную логику режима</summary>
    int Run(string[] args);
}
