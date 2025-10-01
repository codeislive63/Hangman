using Hangman.Application.Abstractions;
using Hangman.Extensions.Configuration;
using Hangman.Domain.Models;

namespace Hangman.Application.Services;

/// <summary>Реализация подготовки игры на базе репозитория слов</summary>
public sealed class GameSetupService(
    IHangmanConfiguration configuration,
    IWordRepository repository) : IGameSetupService
{
    private readonly IHangmanConfiguration _configuration = configuration;
    private readonly IWordRepository _repository = repository;

    /// <inheritdoc />
    public GameSession Create(GameDifficulty? difficulty = null, string? category = null)
    {
        var card = _repository.GetRandomWord(difficulty, category);

        int attempts = _configuration.MapDifficultyToAttempts(card.Difficulty);

        return new GameSession
        {
            Secret = card.Word,
            Category = card.Category,
            Hint = card.Hint,
            Difficulty = card.Difficulty,
            MaxAttempts = attempts,
            HintThreshold = attempts / 2
        };
    }
}