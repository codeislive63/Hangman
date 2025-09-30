using FluentAssertions;
using Hangman.Application.Abstractions;
using Hangman.Application.Services;
using Hangman.Domain.Models;
using Hangman.Extensions.Configuration;

namespace Hangman.Tests;

public class GameSetupServiceTests
{
    private sealed class RepoFake(IEnumerable<WordCard> words) : IWordRepository
    {
        private readonly List<WordCard> _words = [.. words];

        public IReadOnlyCollection<WordCard> GetAllWords() => _words;

        public IReadOnlyCollection<WordCard> GetWordsByDifficulty(GameDifficulty d)
            => [.. _words.Where(w => w.Difficulty == d)];
        
        public WordCard GetRandomWord(GameDifficulty? d = null, string? c = null)
        {
            var q = _words.AsEnumerable();
            if (d.HasValue)
            {
                q = q.Where(w => w.Difficulty == d);
            }

            if (!string.IsNullOrWhiteSpace(c))
            {
                q = q.Where(w => w.Category == c);
            }

            return q.First();
        }
    }

    private sealed class CfgFake : IHangmanConfiguration
    {
        public int MapDifficultyToAttempts(GameDifficulty difficulty)
        {
            return difficulty == GameDifficulty.Easy ? 8
                 : difficulty == GameDifficulty.Normal ? 6
                 : 4;
        }

        public IReadOnlyList<WordCard> WordCards => [];
    }

    /// <summary>Создаёт игровую сессию с выбранной сложностью</summary>
    [Fact]
    public void Create_Session_With_Chosen_Difficulty()
    {
        var repo = new RepoFake([new WordCard("test", "hint", "cat", GameDifficulty.Normal)]);
        var cfg = new CfgFake();
        var svc = new GameSetupService(cfg, repo);

        var s = svc.Create(GameDifficulty.Normal);

        s.Secret.Should().Be("test");
        s.Difficulty.Should().Be(GameDifficulty.Normal);
        s.MaxAttempts.Should().BeGreaterThan(0);
    }
}