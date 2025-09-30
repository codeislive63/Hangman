using FluentAssertions;
using Hangman.Domain.Models;
using Hangman.Extensions.Configuration;
using Hangman.Extensions.Logging;
using Hangman.Infrastructure.Repositories;

namespace Hangman.Tests;

public class WordRepositoryTests
{
    private sealed class CfgFake : IHangmanConfiguration
    {
        private readonly List<WordCard> _cards;

        public CfgFake(IEnumerable<WordCard> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            _cards = [.. cards];
        }

        public int MapDifficultyToAttempts(GameDifficulty difficulty)
        {
            return difficulty == GameDifficulty.Easy ? 8
                 : difficulty == GameDifficulty.Normal ? 6
                 : 4;
        }

        public IReadOnlyList<WordCard> WordCards => _cards;
    }

    /// <summary>Фильтрация и выбор случайного слова работают</summary>
    [Fact]
    public void Filters_And_Random_Works()
    {
        var cfg = new CfgFake(
        [
            new WordCard("alpha","h1","animals", GameDifficulty.Easy),
            new WordCard("beta","h2","plants",  GameDifficulty.Normal)
        ]);

        var repo = new WordRepository(cfg);

        repo.GetAllWords().Should().HaveCount(2);
        repo.GetWordsByDifficulty(GameDifficulty.Normal)
            .Should().ContainSingle(w => w.Word == "beta");
        repo.GetRandomWord(GameDifficulty.Easy, "animals").Word.Should().Be("alpha");
    }
}
