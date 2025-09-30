using FluentAssertions;
using Hangman.Extensions.DependencyInjection;

namespace Hangman.Extensions.Tests;

public class ServiceProviderTests
{
    /// <summary>Enumerable возвращает все регистрации, одиночный резолв берёт последнюю</summary>
    [Fact]
    public void MultipleRegistrations_Enumerable_And_LastWins()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFoo, Foo1>();
        sc.AddSingleton(typeof(IFoo), _ => new Foo2()); // последняя

        var sp = sc.BuildServiceProvider();

        var list = (IEnumerable<IFoo>)sp.GetService(typeof(IEnumerable<IFoo>))!;
        list.Should().HaveCount(2);

        var single = (IFoo)sp.GetService(typeof(IFoo))!;
        single.Should().BeOfType<Foo2>(); // last wins
    }

    /// <summary>Self binding создаёт конкретный класс без явной регистрации</summary>
    [Fact]
    public void SelfBinding_Works_ForConcrete()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var bar = (Bar)sp.GetService(typeof(Bar))!;
        bar.Should().NotBeNull();
    }

    /// <summary>Запрос неизвестного интерфейса возвращает null</summary>
    [Fact]
    public void MissingInterface_ReturnsNull()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        sp.GetService(typeof(IFoo)).Should().BeNull();
    }

    /// <summary>Singleton с готовым экземпляром возвращает тот же инстанс</summary>
    [Fact]
    public void Singleton_Instance_IsReturned_AsIs()
    {
        var inst = new Foo1();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFoo>(inst);

        var sp = sc.BuildServiceProvider();
        var resolved = (IFoo)sp.GetService(typeof(IFoo))!;
        resolved.Should().BeSameAs(inst);
    }

    /// <summary>Singleton фабрика вызывается один раз и кэшируется</summary>
    [Fact]
    public void Singleton_Factory_IsCached()
    {
        int calls = 0;
        var sc = new ServiceCollection();
        sc.AddSingleton(typeof(IFoo), _ => { calls++; return new Foo1(); });

        var sp = sc.BuildServiceProvider();

        var a = (IFoo)sp.GetService(typeof(IFoo))!;
        var b = (IFoo)sp.GetService(typeof(IFoo))!;
        a.Should().BeSameAs(b);
        calls.Should().Be(1);
    }

    /// <summary>Enumerable пуст если регистраций нет</summary>
    [Fact]
    public void Enumerable_Empty_When_No_Registrations()
    {
        var sp = new ServiceCollection().BuildServiceProvider();
        var arr = (Array)sp.GetService(typeof(IEnumerable<IFoo>))!;
        arr.Length.Should().Be(0);
    }

    /// <summary>Self binding корректно резолвит зависимости конструктора</summary>
    [Fact]
    public void SelfBinding_With_Dependencies_Works()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFoo, Foo1>(); // для Qux(IFoo)

        var sp = sc.BuildServiceProvider();

        var qux = (Qux)sp.GetService(typeof(Qux))!;
        qux.Foo.Should().BeOfType<Foo1>();
    }

    /// <summary>Последняя регистрация интерфейса побеждает при одиночном резолве</summary>
    [Fact]
    public void LastRegistration_Wins_For_Single()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFoo, Foo1>();
        sc.AddSingleton<IFoo, Foo2>(); // последняя

        var sp = sc.BuildServiceProvider();

        var foo = (IFoo)sp.GetService(typeof(IFoo))!;
        foo.Should().BeOfType<Foo2>();
    }

    /// <summary>Unresolvable зависимость в self binding приводит к исключению</summary>
    [Fact]
    public void Unresolvable_Dependency_Throws()
    {
        var sp = new ServiceCollection().BuildServiceProvider();

        Action act = () => sp.GetService(typeof(NeedsBaz)); // NeedsBaz(IBaz) без регистрации
        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>Можно зарегистрировать несколько реализаций и получить их как IEnumerable</summary>
    [Fact]
    public void Multiple_Implementations_Are_Resolved_As_IEnumerable()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton(typeof(IController), _ => new ControllerA());
        sc.AddSingleton(typeof(IController), _ => new ControllerB());

        var sp = sc.BuildServiceProvider();

        var list = (IEnumerable<IController>)sp.GetService(typeof(IEnumerable<IController>))!;
        list.Should().ContainSingle(x => x is ControllerA)
            .And.ContainSingle(x => x is ControllerB);
    }

    // ===== тестовые типы =====

    public interface IFoo { }
    public sealed class Foo1 : IFoo { }
    public sealed class Foo2 : IFoo { }

    public sealed class Bar { }

    public sealed class Qux(ServiceProviderTests.IFoo foo)
    {
        public IFoo Foo { get; } = foo;
    }

    public interface IBaz { }
    public sealed class NeedsBaz(IBaz baz)
    {
        public IBaz Baz { get; } = baz;
    }

    public interface IController { }
    public sealed class ControllerA : IController { }
    public sealed class ControllerB : IController { }
}