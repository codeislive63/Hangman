using Hangman.Domain.Models;

namespace Hangman.Application.Abstractions;

/// <summary>Репозиторий слов для игры</summary>
public interface IWordRepository
{
    /// <summary>Возвращает полный список всех слов в словаре</summary>
    IReadOnlyCollection<WordCard> GetAllWords();

    /// <summary>Возвращает список слов указанной сложности</summary>
    IReadOnlyCollection<WordCard> GetWordsByDifficulty(GameDifficulty difficulty);

    /// <summary>Возвращает случайное слово с учётом фильтров</summary>
    WordCard GetRandomWord(GameDifficulty? difficulty, string? category);
}
