using Hangman.Extensions.Infrastructure;
using Hangman.Extensions.Logging;

namespace Hangman.Controllers;

public sealed class NonInteractiveController(ILogger logger) : IController
{
    public int Run(string[] args)
    {
        string secret = args[0];
        string probe = args[1];

        if (secret is null || probe is null || secret.Length != probe.Length)
        {
            logger.LogError("[non-int] null input");
            Console.WriteLine(";NEG");
            return 1;
        }

        var mask = RevealByPosition(secret, probe);
        var result = string.Equals(secret, probe, StringComparison.OrdinalIgnoreCase) ? "POS" : "NEG";
        Console.WriteLine($"{mask};{result}");
        
        return 0;
    }

    private static string RevealByPosition(string secret, string probe)
    {
        var buf = new char[secret.Length];

        for (int i = 0; i < secret.Length; i++)
        {
            var s = secret[i];
            var p = probe[i];
            buf[i] = char.ToLowerInvariant(s) == char.ToLowerInvariant(p) ? s : '*';
        }

        return new string(buf);
    }

    public bool CanHandle(string[] args) => args.Length == 2;
}
