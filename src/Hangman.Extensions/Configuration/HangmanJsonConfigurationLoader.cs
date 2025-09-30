using System.Text;
using System.Text.Json;
using Hangman.Extensions.Logging;
using Hangman.Extensions.Security;

namespace Hangman.Extensions.Configuration;

/// <summary>Загрузка и расшифровка JSON-конфига из .enc файла</summary>
public static class HangmanJsonConfigurationLoader
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static readonly byte[] _aad = Encoding.UTF8.GetBytes("Hangman|appsettings|v1");

    /// <summary>Загружает конфигурацию из зашифрованного файла. Если ключ/файл не найдены — возвращает объекты по умолчанию</summary>
    public static HangmanJsonConfiguration LoadSecure(string encPath, string keyEnv, ILogger? logger = null)
    {
        if (!File.Exists(encPath))
        {
            logger?.LogWarning("[config] file not found: " + encPath);
            return new HangmanJsonConfiguration();
        }

        string? keyB64 = Environment.GetEnvironmentVariable(keyEnv);

        if (string.IsNullOrWhiteSpace(keyB64))
        {
            logger?.LogError("[config] env var is not set: " + keyEnv);
            return new HangmanJsonConfiguration();
        }

        string? json;

        try
        {
            byte[] key = Convert.FromBase64String(keyB64);
            string b64 = File.ReadAllText(encPath).Trim();
            json = AesGcmEncryptor.DecryptFromBase64(b64, key, _aad);
            logger?.LogInformation("[config] decrypted: " + encPath);
        }
        catch (Exception ex)
        {
            logger?.LogError("[config] decrypt failed", ex);
            return new HangmanJsonConfiguration();
        }

        return DeserializeOrDefault(json, logger);
    }

    private static HangmanJsonConfiguration DeserializeOrDefault(string? json, ILogger? logger)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            logger?.LogWarning("[config] empty json, using defaults");
            return new HangmanJsonConfiguration();
        }

        try
        {
            var cfg = JsonSerializer.Deserialize<HangmanJsonConfiguration>(json, _options);
            return cfg ?? new HangmanJsonConfiguration();
        }
        catch (Exception ex)
        {
            logger?.LogError("[config] json parse failed", ex);
            return new HangmanJsonConfiguration();
        }
    }
}
