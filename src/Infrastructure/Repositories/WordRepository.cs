using Hangman.Application.Abstractions;
using Hangman.Domain.Models;
using Hangman.Extensions.Configuration;

namespace Hangman.Infrastructure.Repositories;

/// <summary>Реализация репозитория слов</summary>
public sealed class WordRepository(IHangmanConfiguration configuration) : IWordRepository
{
    private readonly IReadOnlyCollection<WordCard> _allWords = [
        .. configuration.WordCards.Select(w => new WordCard(
            w.Word,
            w.Hint,
            w.Category,
            w.Difficulty
        ))
    ];

    /// <inheritdoc />
    public IReadOnlyCollection<WordCard> GetAllWords() => _allWords;

    /// <inheritdoc />
    public IReadOnlyCollection<WordCard> GetWordsByDifficulty(GameDifficulty difficulty)
        => [.. _allWords.Where(x => x.Difficulty == difficulty)];

    /// <inheritdoc />
    public WordCard GetRandomWord(GameDifficulty? difficulty = null, string? category = null)
    {
        IEnumerable<WordCard> query = _allWords;

        if (difficulty != null)
        {
            query = query.Where(x => x.Difficulty == difficulty);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        List<WordCard> list = [.. query];

        return list.Count == 0
            ? throw new InvalidOperationException("No words match the specified filter.")
            : list[Random.Shared.Next(list.Count)];
    }
}
