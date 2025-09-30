using Hangman.Domain.Models;

namespace Hangman.Extensions.Infrastructure;

/// <summary>Интерфейс консольного UI для игры</summary>
public interface IView
{
    /// <summary>Очистить экран</summary>
    void Clear();

    /// <summary>Отрисовать игровой кадр</summary>
    void RenderFrame(
        string category, GameDifficulty difficulty, int maxAttempts, string hint,
        string maskedWord, int mistakes, int maxAttemptsAgain, IReadOnlyCollection<char> wrongLetters
    );

    /// <summary>Запросить сложность</summary>
    GameDifficulty PromptDifficulty(GameDifficulty @default = GameDifficulty.Normal);

    /// <summary>Запросить одну букву</summary>
    string PromptLetter();

    /// <summary>Показать заголовок игры перед стартом раунда</summary>
    void ShowHeader(
        string category,
        GameDifficulty difficulty,
        int maxAttempts,
        string hint
    );

    /// <summary>Показать победу (с раскрытием загаданного слова)</summary>
    void ShowWin(string secret);

    /// <summary>Показать поражение (с раскрытием загаданного слова)</summary>
    void ShowLose(string secret);

    /// <summary>Показать произвольное сообщение</summary>
    void ShowMessage(string text);
}
