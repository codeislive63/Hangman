namespace Hangman.Domain.Models;

/// <summary>Карточка слова</summary>
public sealed record WordCard(
    string Word,
    string Hint,
    string Category,
    GameDifficulty Difficulty
);