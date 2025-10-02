using Hangman.Application.Abstractions;
using Hangman.Domain.Engine;
using Hangman.Domain.Models;
using Hangman.Extensions.Infrastructure;
using Hangman.Views;

namespace Hangman.Controllers;

/// <summary>Интерактивный режим: цикл «показ кадра → ввод → ход движка».</summary>
public sealed class InteractiveController(
    IGameSetupService gameSetupService,
    IConsoleView view) : IController
{
    private readonly IGameSetupService _gameSetupService = gameSetupService;
    private readonly IConsoleView _view = view;

    public bool CanHandle(string[] args) => args.Length != 2;

    public int Run(string[] args)
    {
        _view.Clear();
        var difficulty = _view.PromptDifficulty();
        var session = _gameSetupService.Create(difficulty);

        var engine = new HangmanEngine(session.Secret, session.MaxAttempts);

        while (true)
        {
            int remaining = Math.Max(0, session.HintThreshold - engine.MistakeCount);

            static string errorsGenitive(int n) => (n % 10 == 1 && n % 100 != 11) ? "ошибки" : "ошибок";

            string hintText = remaining == 0
                ? session.Hint
                : $"(подсказка после {remaining} {errorsGenitive(remaining)})";

            _view.RenderFrame(
                session.Category, session.Difficulty, session.MaxAttempts, hintText,
                engine.RevealedWord, engine.MistakeCount, engine.MaxMistakes, engine.MissedLetters
            );

            var input = _view.PromptLetter();
            var result = engine.MakeGuess(input);

            switch (result)
            {
                case GuessResult.Invalid:
                    _view.ShowMessage("Введите РОВНО одну букву.");
                    continue;

                case GuessResult.Repeat:
                    _view.ShowMessage("Эта буква уже вводилась.");
                    continue;

                case GuessResult.Hit:
                    _view.ShowMessage("Есть!");
                    break;

                case GuessResult.Miss:
                    _view.ShowMessage("Мимо.");
                    break;

                case GuessResult.Win:
                    _view.RenderFrame(
                        session.Category, session.Difficulty, session.MaxAttempts, session.Hint,
                        engine.RevealedWord, engine.MistakeCount, engine.MaxMistakes, engine.MissedLetters
                    );

                    _view.ShowWin(session.Secret);

                    return 0;

                case GuessResult.Lose:
                    _view.RenderFrame(
                        session.Category, session.Difficulty, session.MaxAttempts, session.Hint,
                        engine.RevealedWord, engine.MistakeCount, engine.MaxMistakes, engine.MissedLetters
                    );

                    _view.ShowLose(session.Secret);

                    return 0;
            }
        }
    }
}
