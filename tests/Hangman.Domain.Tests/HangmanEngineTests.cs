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

    /// <summary>
    /// Основные сценарии угадывания: Invalid → Repeat → Hit → Miss → Win → Lose.
    /// Используем только кириллицу, так как движок работает с русским алфавитом.
    /// </summary>
    [Fact]
    public void MakeGuess_Invalid_Repeat_Hit_Miss_Win_Lose()
    {
        var eng = new HangmanEngine("алла", 2);

        eng.MakeGuess("").Should().Be(GuessResult.Invalid);
        eng.MakeGuess("аб").Should().Be(GuessResult.Invalid);
        eng.MakeGuess('а').Should().Be(GuessResult.Hit);
        eng.MakeGuess('А').Should().Be(GuessResult.Repeat);
        eng.MakeGuess('х').Should().Be(GuessResult.Miss);
        eng.MakeGuess('л').Should().Be(GuessResult.Win);

        var lose = new HangmanEngine("да", 1);
        lose.MakeGuess('х').Should().Be(GuessResult.Lose);
    }

    /// <summary>
    /// Конструктор должен отклонять слова без кириллицы
    /// </summary>
    [Fact]
    public void Ctor_Rejects_Secrets_Without_Cyrillic()
    {
        Action latinOnly = () => new HangmanEngine("hello", 5);
        Action digitsOnly = () => new HangmanEngine("12345", 5);
        Action symbolsOnly = () => new HangmanEngine("@#!$", 5);

        latinOnly.Should().Throw<ArgumentException>()
            .WithMessage("*Cyrillic*");
        digitsOnly.Should().Throw<ArgumentException>()
            .WithMessage("*Cyrillic*");
        symbolsOnly.Should().Throw<ArgumentException>()
            .WithMessage("*Cyrillic*");
    }

    /// <summary>
    /// Ввод латинских символов считается некорректным.
    /// </summary>
    [Fact]
    public void LatinLetters_Are_Invalid()
    {
        var eng = new HangmanEngine("кот", 3);

        eng.MakeGuess('a').Should().Be(GuessResult.Invalid);
        eng.MakeGuess('Z').Should().Be(GuessResult.Invalid);
        eng.MakeGuess('x').Should().Be(GuessResult.Invalid);
    }

    /// <summary>Счётчик оставшихся попыток уменьшается только на новые промахи, но не на повторы</summary>
    [Fact]
    public void AttemptsRemaining_Decreases_OnNewMiss_And_NotOnRepeat()
    {
        var eng = new HangmanEngine("абв", 3);

        eng.AttemptsRemaining.Should().Be(3);
        eng.MakeGuess('х').Should().Be(GuessResult.Miss);
        eng.AttemptsRemaining.Should().Be(2);

        eng.MakeGuess('Х').Should().Be(GuessResult.Repeat);
        eng.AttemptsRemaining.Should().Be(2);
    }

    /// <summary>Ввод не букв считается некорректным</summary>
    [Fact]
    public void NonLetter_Input_IsInvalid()
    {
        var eng = new HangmanEngine("абв", 3);

        eng.MakeGuess('1').Should().Be(GuessResult.Invalid);
        eng.MakeGuess('$').Should().Be(GuessResult.Invalid);
        eng.MakeGuess(" ").Should().Be(GuessResult.Invalid);
        eng.MakeGuess("\t").Should().Be(GuessResult.Invalid);
    }

    /// <summary>Повторно угаданная правильная буква не изменяет состояние и счётчики ошибок</summary>
    [Fact]
    public void Repeat_Correct_Letter_DoesNotChange_State()
    {
        var eng = new HangmanEngine("абв", 3);

        eng.MakeGuess('а').Should().Be(GuessResult.Hit);
        var mistakesBefore = eng.MistakeCount;

        eng.MakeGuess('А').Should().Be(GuessResult.Repeat);
        eng.MistakeCount.Should().Be(mistakesBefore);
    }

    /// <summary>В строке небуквенные символы не требуют угадывания для победы</summary>
    [Fact]
    public void Secret_With_NonLetters_DoesNotRequire_Guessing_Them_For_Win()
    {
        var eng = new HangmanEngine("а-а 42", 5);

        eng.MakeGuess('а');
        eng.IsWon.Should().BeTrue();
        eng.RevealedWord.Length.Should().Be("а-а 42".Length);
    }

    /// <summary>Проигрыш наступает при достижении лимита ошибок</summary>
    [Fact]
    public void Lose_Exactly_On_Limit()
    {
        var eng = new HangmanEngine("аб", 2);

        eng.MakeGuess('х').Should().Be(GuessResult.Miss);
        eng.IsLost.Should().BeFalse();

        eng.MakeGuess('у').Should().Be(GuessResult.Lose);
        eng.IsLost.Should().BeTrue();
        eng.AttemptsRemaining.Should().Be(0);
    }

    /// <summary>Список ошибочных букв хранит уникальные символы в нижнем регистре</summary>
    [Fact]
    public void MissedLetters_Are_Distinct_And_Lowercased()
    {
        var eng = new HangmanEngine("абв", 5);

        eng.MakeGuess('Х').Should().Be(GuessResult.Miss);
        eng.MakeGuess('х').Should().Be(GuessResult.Repeat);

        eng.MissedLetters.Should().ContainSingle().And.Contain('х');
    }
}
