using Hangman.Domain.Models;

namespace Hangman.Domain.Engine;

/// <summary>
/// Движок виселицы. Хранит состояние, принимает буквы, считает промахи/победы
/// </summary>
public sealed class HangmanEngine
{
    private readonly string _secretOriginal;
    private readonly string _secretLower;
    private readonly HashSet<char> _correctLetters = [];
    private readonly HashSet<char> _missedLetters = [];
    private readonly HashSet<char> _distinctSecretLetters;

    public int MaxMistakes { get; }
    public int MistakeCount => _missedLetters.Count;
    public int AttemptsRemaining => MaxMistakes - MistakeCount;
    public IReadOnlyCollection<char> MissedLetters => _missedLetters;

    public string RevealedWord => string.Concat(
        _secretOriginal.Select(ch => _correctLetters.Contains(char.ToLowerInvariant(ch)) ? ch : '_')
    );

    public bool IsWon => _distinctSecretLetters.IsSubsetOf(_correctLetters);
    public bool IsLost => MistakeCount >= MaxMistakes;

    public HangmanEngine(string secret, int maxMistakes)
    {
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new ArgumentException("Secret word must be non-empty.", nameof(secret));
        }

        _secretOriginal = secret;
        _secretLower = secret.ToLowerInvariant();

        MaxMistakes = Math.Max(1, maxMistakes);

        _distinctSecretLetters = [.. _secretLower
            .Where(char.IsLetter)
            .Select(char.ToLowerInvariant)
        ];
    }

    /// <summary>Обрабатывает строковый ввод, ожидая 1 символ.</summary>
    public GuessResult MakeGuess(string? input)
    {
        return string.IsNullOrWhiteSpace(input) || input.Trim().Length != 1
            ? GuessResult.Invalid
            : MakeGuess(char.ToLowerInvariant(input.Trim()[0]));
    }

    /// <summary>Обрабатывает ввод одной буквы</summary>
    public GuessResult MakeGuess(char ch)
    {
        if (!char.IsLetter(ch))
        {
            return GuessResult.Invalid;
        }

        var letter = char.ToLowerInvariant(ch);

        if (_correctLetters.Contains(letter) || _missedLetters.Contains(letter))
        {
            return GuessResult.Repeat;
        }

        if (_secretLower.Contains(letter))
        {
            _correctLetters.Add(letter);
            return IsWon ? GuessResult.Win : GuessResult.Hit;
        }

        _missedLetters.Add(letter);

        return IsLost ? GuessResult.Lose : GuessResult.Miss;
    }
}
