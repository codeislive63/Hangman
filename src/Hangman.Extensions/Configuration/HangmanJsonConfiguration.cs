using Hangman.Domain.Models;
using System.Text.Json.Serialization;

namespace Hangman.Extensions.Configuration;


/// <summary>POCO-модель конфигурации, совместимая с JSON</summary>
public sealed class HangmanJsonConfiguration : IHangmanConfiguration
{
    public DifficultyConfiguration GameDifficulty { get; init; } = new();

    [JsonPropertyName("Words")]
    public List<WordItem> WordsRaw { get; init; } = [];

    /// <inheritdoc />
    public IReadOnlyList<WordCard> WordCards => [.. WordsRaw.Select(w =>
    {
        if (!Enum.TryParse<GameDifficulty>(w.DifficultyRaw, true, out var parsed))
        {
            parsed = Domain.Models.GameDifficulty.Normal;
        }

        return new WordCard(
            w.Word,
            string.IsNullOrWhiteSpace(w.Hint) ? string.Empty : w.Hint,
            w.Category,
            parsed
        );
    })];

    /// <inheritdoc />
    public int MapDifficultyToAttempts(GameDifficulty difficulty) => difficulty switch
    {
        Domain.Models.GameDifficulty.Easy => GameDifficulty.Easy.AttemptCount,
        Domain.Models.GameDifficulty.Normal => GameDifficulty.Normal.AttemptCount,
        Domain.Models.GameDifficulty.Hard => GameDifficulty.Hard.AttemptCount,
        _ => GameDifficulty.Normal.AttemptCount
    };
}

public sealed class DifficultyConfiguration
{
    public DifficultyLevel Easy { get; init; } = new();

    public DifficultyLevel Normal { get; init; } = new();

    public DifficultyLevel Hard { get; init; } = new();
}

public sealed class DifficultyLevel
{
    public int AttemptCount { get; init; }
}

/// <summary>Элемент словаря слов</summary>
public sealed class WordItem
{
    public required string Word { get; init; }

    public string Hint { get; init; } = "";

    public required string Category { get; init; }


    [JsonPropertyName("Difficulty")]
    public string DifficultyRaw { get; init; } = "";
}
