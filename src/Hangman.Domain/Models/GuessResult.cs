namespace Hangman.Domain.Models;

/// <summary>Итог проверки попытки угадывания</summary>
public enum GuessResult
{
    Invalid,
    Repeat,
    Hit,
    Miss,
    Win,
    Lose
}
