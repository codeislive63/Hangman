using Hangman.Domain.Models;
using Hangman.Extensions.Infrastructure;

namespace Hangman.Views;

/// <summary>Консольная реализация интерфейса отображения игры</summary>
public sealed class ConsoleView : IView
{
    private static readonly string[] Frames =
    [
        """
         +---+
         |   |
             |
             |
             |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
             |
             |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
         |   |
             |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
        /|   |
             |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
        /|\  |
             |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
        /|\  |
        /    |
             |
        =========
        """,
        """
         +---+
         |   |
         O   |
        /|\  |
        / \  |
             |
        =========
        """
    ];

    /// <inheritdoc />
    public void Clear() => Console.Clear();

    /// <inheritdoc />
    public GameDifficulty PromptDifficulty(GameDifficulty @default = GameDifficulty.Normal)
    {
        Console.WriteLine("Выберите сложность: [E]asy / [N]ormal / [H]ard");
        Console.Write($"По умолчанию {@default}: ");

        while (true)
        {
            var key = Console.ReadKey(intercept: true).Key;
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.E: return GameDifficulty.Easy;
                case ConsoleKey.N: return GameDifficulty.Normal;
                case ConsoleKey.H: return GameDifficulty.Hard;
                case ConsoleKey.Enter: return @default;
                default:
                    Console.WriteLine("Нажмите E / N / H (или Enter для значения по умолчанию).");
                    continue;
            }
        }
    }

    /// <inheritdoc />
    public string PromptLetter()
    {
        Console.Write("Введите одну букву: ");
        return Console.ReadLine() ?? string.Empty;
    }

    /// <inheritdoc />
    public void RenderFrame(string category, GameDifficulty difficulty, int maxAttempts, string hint, string maskedWord, int mistakes, int maxAttemptsAgain, IReadOnlyCollection<char> wrongLetters)
    {
        Clear();
        ShowHeader(category, difficulty, maxAttempts, hint);

        var frameIndex = GetFrameIndex(mistakes, maxAttemptsAgain);
        var frame = Frames[frameIndex];

        Console.WriteLine(frame);
        Console.WriteLine($"Слово:   {maskedWord}");
        Console.WriteLine($"Ошибок:  {mistakes}/{maxAttemptsAgain}");
        Console.WriteLine($"Промахи: {string.Join(' ', wrongLetters.OrderBy(c => c))}");
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void ShowHeader(string category, GameDifficulty difficulty, int maxAttempts, string hint)
    {
        Console.WriteLine($"Категория: {category} | Сложность: {difficulty} | Попыток: {maxAttempts}");
        Console.WriteLine($"Подсказка: {hint}");
        Console.WriteLine("Правила: вводите одну букву за ход. Регистр не важен.");
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void ShowMessage(string text) => Console.WriteLine(text);

    /// <inheritdoc />
    public void ShowWin(string secret) => Console.WriteLine($"Победа! Загаданное слово: {secret}");

    /// <inheritdoc />
    public void ShowLose(string secret) => Console.WriteLine($"Поражение. Загаданное слово: {secret}");

    private static int GetFrameIndex(int mistakes, int maxAttempts)
    {
        var denom = Math.Max(1, maxAttempts);

        var idx = (int)Math.Round(
            (double)mistakes * (Frames.Length - 1) / denom,
            MidpointRounding.AwayFromZero);

        return Math.Clamp(idx, 0, Frames.Length - 1);
    }
}
