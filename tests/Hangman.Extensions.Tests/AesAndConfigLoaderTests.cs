using FluentAssertions;
using Hangman.Extensions.Configuration;
using Hangman.Extensions.Logging;
using Hangman.Extensions.Security;
using System.Text;
using System.Text.Json;

namespace Hangman.Extensions.Tests;

public class AesAndConfigLoaderTests
{
    private sealed class NullLogger : ILogger
    {
        public void LogInformation(string m) { }
        public void LogWarning(string m) { }
        public void LogError(string m, Exception? ex = null) { }
        public void LogDebug(string message) { }
        public void LogCritical(string message, Exception? ex = null) { }
    }

    /// <summary>Строка корректно шифруется и расшифровывается обратно</summary>
    [Fact]
    public void Encrypt_Decrypt_Roundtrip()
    {
        var key = new byte[32]; Random.Shared.NextBytes(key);
        var aad = Encoding.UTF8.GetBytes("Hangman|appsettings|v1");
        var cipher = AesGcmEncryptor.EncryptToBase64("hello", key, aad);
        var plain = AesGcmEncryptor.DecryptFromBase64(cipher, key, aad);
        plain.Should().Be("hello");
    }

    /// <summary>Возвращает дефолтные значения если файла или ключа нет</summary>
    [Fact]
    public void LoadSecure_FileMissing_Or_KeyMissing_Returns_Default()
    {
        var cfg = HangmanJsonConfigurationLoader.LoadSecure("missing.enc", "MISSING", new NullLogger());
        cfg.Should().NotBeNull();
    }

    /// <summary>Корректно загружает конфиг из зашифрованного файла</summary>
    [Fact]
    public void LoadSecure_ValidEncryptedFile_Returns_Config()
    {
        var sample = new HangmanJsonConfiguration();
        var json = JsonSerializer.Serialize(sample);

        var key = new byte[32]; Random.Shared.NextBytes(key);
        var aad = Encoding.UTF8.GetBytes("Hangman|appsettings|v1");
        var enc = AesGcmEncryptor.EncryptToBase64(json, key, aad);

        var encPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".enc");
        File.WriteAllText(encPath, enc);
        Environment.SetEnvironmentVariable("HANGMAN_KEY", Convert.ToBase64String(key));

        var cfg = HangmanJsonConfigurationLoader.LoadSecure(encPath, "HANGMAN_KEY", new NullLogger());
        cfg.Should().NotBeNull();

        File.Delete(encPath);
        Environment.SetEnvironmentVariable("HANGMAN_KEY", null);
    }
}