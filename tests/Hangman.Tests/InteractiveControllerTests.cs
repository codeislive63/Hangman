using FluentAssertions;
using Hangman.Application.Abstractions;
using Hangman.Controllers;
using Hangman.Domain.Models;
using Hangman.Views;
using NSubstitute;

namespace Hangman.Tests;

public class InteractiveControllerTests
{
    /// <summary>Проверяет, что при правильных ходах игра завершается победой и код выхода 0</summary>
    [Fact]
    public void Win_Flow_Shows_Win_And_Returns_0()
    {
        var view = Substitute.For<IConsoleView>();
        view.PromptDifficulty(Arg.Any<GameDifficulty>()).Returns(GameDifficulty.Easy);
        view.PromptLetter().Returns("a", "b");

        var setup = Substitute.For<IGameSetupService>();
        setup.Create(GameDifficulty.Easy).Returns(new GameSession
        {
            Secret = "ab",
            Difficulty = GameDifficulty.Easy,
            MaxAttempts = 5,
            HintThreshold = 1,
            Category = "t",
            Hint = "h"
        });

        var ctrl = new InteractiveController(setup, view);

        var code = ctrl.Run([]);

        code.Should().Be(0);
        view.Received().ShowWin("ab");
    }

    /// <summary>Показывает сообщения валидации и поведение repeat, затем проигрыш</summary>
    [Fact]
    public void Validation_Messages_Are_Shown_And_Lose_Path_Works()
    {
        var view = Substitute.For<IConsoleView>();
        view.PromptDifficulty(Arg.Any<GameDifficulty>()).Returns(GameDifficulty.Normal);
        // invalid -> miss(b) -> repeat(b) -> miss(x) => lose при MaxAttempts=2
        view.PromptLetter().Returns("", "b", "b", "x");

        var setup = Substitute.For<IGameSetupService>();
        setup.Create(GameDifficulty.Normal).Returns(new GameSession
        {
            Secret = "a",
            Difficulty = GameDifficulty.Normal,
            MaxAttempts = 2,
            HintThreshold = 0,
            Category = "c",
            Hint = "h"
        });

        var ctrl = new InteractiveController(setup, view);

        var code = ctrl.Run([]);

        code.Should().Be(0);
        view.Received().ShowMessage(Arg.Is<string>(s => s.Contains("РОВНО одну букву")));
        view.Received().ShowMessage(Arg.Is<string>(s => s.Contains("уже вводилась")));
        view.Received().ShowLose("a");
    }
}
