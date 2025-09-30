using FluentAssertions;
using Hangman.Domain.Engine;
using Hangman.Domain.Models;

namespace Hangman.Domain.Tests;

public class HangmanEngineTests
{
    /// <summary>Конструктор: пустая строка бросает ArgumentException</summary>
    [Fact]
    public void Ctor_EmptySecret_Throws()
    {
        var act = () => new HangmanEngine("", 5);
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>Начальное состояние движка корректное: счётчики, слово скрыто, не выиграл/не проиграл.</summary>
    [Fact]
    public void Initial_State_IsCorrect()
    {
        var eng = new HangmanEngine("Тест", 3);
        eng.MaxMistakes.Should().Be(3);
        eng.MistakeCount.Should().Be(0);
        eng.IsWon.Should().BeFalse();
        eng.IsLost.Should().BeFalse();
        eng.RevealedWord.Should().Be("____");
    }

    /// <summary>Основные сценарии угадывания: Invalid → Repeat → Hit → Miss → Win → Lose.</summary>
    [Fact]
    public void MakeGuess_Invalid_Repeat_Hit_Miss_Win_Lose()
    {
        var eng = new HangmanEngine("abba", 2);

        eng.MakeGuess("").Should().Be(GuessResult.Invalid);
        eng.MakeGuess("ab").Should().Be(GuessResult.Invalid);
        eng.MakeGuess('a').Should().Be(GuessResult.Hit);
        eng.MakeGuess('A').Should().Be(GuessResult.Repeat);
        eng.MakeGuess('x').Should().Be(GuessResult.Miss);
        eng.MakeGuess('b').Should().Be(GuessResult.Win);

        var lose = new HangmanEngine("ab", 1);
        lose.MakeGuess('x').Should().Be(GuessResult.Lose);
    }

    /// <summary>Счётчик оставшихся попыток уменьшается только на новые промахи, но не на повторы</summary>
    [Fact]
    public void AttemptsRemaining_Decreases_OnNewMiss_And_NotOnRepeat()
    {
        var eng = new HangmanEngine("abc", 3);

        eng.AttemptsRemaining.Should().Be(3);
        eng.MakeGuess('x').Should().Be(GuessResult.Miss);
        eng.AttemptsRemaining.Should().Be(2);

        eng.MakeGuess('X').Should().Be(GuessResult.Repeat);
        eng.AttemptsRemaining.Should().Be(2);
    }

    /// <summary>Ввод не букв считается некорректным</summary>
    [Fact]
    public void NonLetter_Input_IsInvalid()
    {
        var eng = new HangmanEngine("abc", 3);

        eng.MakeGuess('1').Should().Be(GuessResult.Invalid);
        eng.MakeGuess('$').Should().Be(GuessResult.Invalid);
        eng.MakeGuess(" ").Should().Be(GuessResult.Invalid);
        eng.MakeGuess("\t").Should().Be(GuessResult.Invalid);
    }

    /// <summary>Повторно угаданная правильная буква не изменяет состояние и счётчики ошибок</summary>
    [Fact]
    public void Repeat_Correct_Letter_DoesNotChange_State()
    {
        var eng = new HangmanEngine("abc", 3);

        eng.MakeGuess('a').Should().Be(GuessResult.Hit);
        var mistakesBefore = eng.MistakeCount;

        eng.MakeGuess('A').Should().Be(GuessResult.Repeat);
        eng.MistakeCount.Should().Be(mistakesBefore);
    }

    /// <summary>В строке небуквенные символы не требуют угадывания для победы</summary>
    [Fact]
    public void Secret_With_NonLetters_DoesNotRequire_Guessing_Them_For_Win()
    {
        var eng = new HangmanEngine("re-entry 42", 5);

        foreach (var ch in new[] { 'r', 'e', 'n', 't', 'y' })
        {
            eng.MakeGuess(ch);
        }

        eng.IsWon.Should().BeTrue();
        eng.RevealedWord.Length.Should().Be("re-entry 42".Length);
    }

    /// <summary>Проигрыш наступает при достижении лимита ошибок</summary>
    [Fact]
    public void Lose_Exactly_On_Limit()
    {
        var eng = new HangmanEngine("ab", 2);

        eng.MakeGuess('x').Should().Be(GuessResult.Miss);
        eng.IsLost.Should().BeFalse();

        eng.MakeGuess('y').Should().Be(GuessResult.Lose);
        eng.IsLost.Should().BeTrue();
        eng.AttemptsRemaining.Should().Be(0);
    }

    /// <summary>Список ошибочных букв хранит уникальные символы в нижнем регистре</summary>
    [Fact]
    public void MissedLetters_Are_Distinct_And_Lowercased()
    {
        var eng = new HangmanEngine("abc", 5);

        eng.MakeGuess('X').Should().Be(GuessResult.Miss);
        eng.MakeGuess('x').Should().Be(GuessResult.Repeat);

        eng.MissedLetters.Should().ContainSingle().And.Contain('x');
    }
}
