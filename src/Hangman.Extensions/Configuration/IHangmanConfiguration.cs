using Hangman.Domain.Models;

namespace Hangman.Extensions.Configuration;

/// <summary>Интерфейс конфигурации игры</summary>
public interface IHangmanConfiguration
{
    /// <summary>Преобразует уровень сложности в количество доступных попыток</summary>
    int MapDifficultyToAttempts(GameDifficulty difficulty);

    /// <summary>Коллекция слов, доступных в игре (с подсказками, категориями и уровнем сложности)</summary>
    IReadOnlyList<WordCard> WordCards { get; }
}