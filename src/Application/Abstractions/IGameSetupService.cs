using Hangman.Domain.Models;

namespace Hangman.Application.Abstractions;

/// <summary>Сервис подготовки новой игры</summary>
public interface IGameSetupService
{
    /// <summary>Создаёт параметры игровой сессии по выбранной сложности</summary>
    GameSession Create(GameDifficulty? difficulty = null, string? category = null);
}
