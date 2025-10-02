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
}
