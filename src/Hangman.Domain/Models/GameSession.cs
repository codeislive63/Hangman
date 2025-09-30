namespace Hangman.Domain.Models;

/// <summary>Модель игровой сессии</summary>
public sealed class GameSession
{
    public required string Secret { get; init; }

    public required string Category { get; init; }

    public required string Hint { get; init; }

    public required GameDifficulty Difficulty { get; init; }

    public required int MaxAttempts { get; init; }

    public required int HintThreshold { get; init; }
}
