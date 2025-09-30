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

    private static readonly string _builtinJson = """
    {
      "words": [
        // Easy (10)
        { "word": "арбуз",        "hint": "Большая зелёная ягода с красной мякотью",    "category": "Еда",       "difficultyRaw": "Easy" },
        { "word": "бублик",       "hint": "Круглый хлебец с дыркой посередине",         "category": "Еда",       "difficultyRaw": "Easy" },
        { "word": "феникс",       "hint": "Птица, возрождающаяся из пепла",             "category": "Мифология", "difficultyRaw": "Easy" },
        { "word": "ключница",     "hint": "Аксессуар для хранения ключей",              "category": "Предметы",  "difficultyRaw": "Easy" },
        { "word": "сфинкс",       "hint": "Лев с человеческим лицом",                   "category": "Мифология", "difficultyRaw": "Easy" },
        { "word": "ежевика",      "hint": "Чёрная кисло-сладкая ягода",                 "category": "Еда",       "difficultyRaw": "Easy" },
        { "word": "самокат",      "hint": "Транспорт для детей и взрослых",             "category": "Предметы",  "difficultyRaw": "Easy" },
        { "word": "енот",         "hint": "Животное с «маской» на морде",               "category": "Животные",  "difficultyRaw": "Easy" },
        { "word": "лягушка",      "hint": "Зелёное земноводное",                        "category": "Животные",  "difficultyRaw": "Easy" },
        { "word": "кувшин",       "hint": "Сосуд для воды или вина",                    "category": "Предметы",  "difficultyRaw": "Easy" },

        // Normal (10)
        { "word": "мангуст",      "hint": "Зверёк, враг змей",                          "category": "Животные",  "difficultyRaw": "Normal" },
        { "word": "василиск",     "hint": "Мифическое существо, убивающее взглядом",    "category": "Мифология", "difficultyRaw": "Normal" },
        { "word": "гранат",       "hint": "Фрукт с множеством зёрен",                   "category": "Еда",       "difficultyRaw": "Normal" },
        { "word": "самурай",      "hint": "Воин в Японии",                              "category": "Мифология", "difficultyRaw": "Normal" },
        { "word": "фонарь",       "hint": "Источает свет в темноте",                    "category": "Предметы",  "difficultyRaw": "Normal" },
        { "word": "зебра",        "hint": "Полосатое животное из Африки",               "category": "Животные",  "difficultyRaw": "Normal" },
        { "word": "пельмени",     "hint": "Тесто с мясной начинкой",                    "category": "Еда",       "difficultyRaw": "Normal" },
        { "word": "пергамент",    "hint": "Материал для письма в древности",            "category": "Предметы",  "difficultyRaw": "Normal" },
        { "word": "цербер",       "hint": "Трёхголовый пёс Аида",                       "category": "Мифология", "difficultyRaw": "Normal" },
        { "word": "кактус",       "hint": "Растение пустыни с колючками",               "category": "Предметы",  "difficultyRaw": "Normal" },

        // Hard (10)
        { "word": "хамелеон",     "hint": "Меняет цвет кожи",                           "category": "Животные",  "difficultyRaw": "Hard" },
        { "word": "амброзия",     "hint": "Пища богов в мифах",                         "category": "Мифология", "difficultyRaw": "Hard" },
        { "word": "зазеркалье",   "hint": "Мир по ту сторону отражения",                "category": "Мифология", "difficultyRaw": "Hard" },
        { "word": "тмин",         "hint": "Пряность с характерным ароматом",            "category": "Еда",       "difficultyRaw": "Hard" },
        { "word": "луковица",     "hint": "Основа для борща и супа",                    "category": "Еда",       "difficultyRaw": "Hard" },
        { "word": "астролябия",   "hint": "Древний астрономический прибор",             "category": "Предметы",  "difficultyRaw": "Hard" },
        { "word": "фолиант",      "hint": "Очень большая книга",                        "category": "Предметы",  "difficultyRaw": "Hard" },
        { "word": "гарпия",       "hint": "Женщина-птица из мифов",                     "category": "Мифология", "difficultyRaw": "Hard" },
        { "word": "мангустин",    "hint": "Экзотический фиолетовый фрукт",              "category": "Еда",       "difficultyRaw": "Hard" },
        { "word": "аксолотль",    "hint": "Редкое мексиканское земноводное",            "category": "Животные",  "difficultyRaw": "Hard" }
      ]
    }
""";


    private static readonly byte[] _aad = Encoding.UTF8.GetBytes("Hangman|appsettings|v1");

    /// <summary>Загружает конфигурацию из зашифрованного файла. Если ключ/файл не найдены — возвращает объекты по умолчанию</summary>
    public static HangmanJsonConfiguration LoadSecure(string encPath, string keyEnv, ILogger? logger = null)
    {
        if (!File.Exists(encPath))
        {
            logger?.LogWarning("[config] file not found: " + encPath);
            return DeserializeOrDefault(_builtinJson, logger);
        }

        string? keyB64 = Environment.GetEnvironmentVariable(keyEnv);

        if (string.IsNullOrWhiteSpace(keyB64))
        {
            logger?.LogError("[config] env var is not set: " + keyEnv);
            return DeserializeOrDefault(_builtinJson, logger);
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
