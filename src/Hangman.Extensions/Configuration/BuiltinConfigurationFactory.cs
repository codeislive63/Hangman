namespace Hangman.Extensions.Configuration;

public static class BuiltinConfigurationFactory
{
    public static HangmanJsonConfiguration Create()
    {
        return new HangmanJsonConfiguration
        {
            GameDifficulty = new()
            {
                Easy = new DifficultyLevel { AttemptCount = 8 },
                Normal = new DifficultyLevel { AttemptCount = 6 },
                Hard = new DifficultyLevel { AttemptCount = 4 }
            },
            WordsRaw =
            [
                // Easy (10)
                new WordItem { Word = "арбуз",    Hint = "большая зелёная ягода с красной мякотью", Category = "Еда", DifficultyRaw = "Easy" },
                new WordItem { Word = "бублик",   Hint = "круглый хлебец с дыркой посередине",     Category = "Еда", DifficultyRaw = "Easy" },
                new WordItem { Word = "феникс",   Hint = "птица, возрождающаяся из пепла",         Category = "Мифология", DifficultyRaw = "Easy" },
                new WordItem { Word = "ключница", Hint = "аксессуар для хранения ключей",          Category = "Предметы",  DifficultyRaw = "Easy" },
                new WordItem { Word = "сфинкс",   Hint = "лев с человеческим лицом",               Category = "Мифология", DifficultyRaw = "Easy" },
                new WordItem { Word = "ежевика",  Hint = "чёрная кисло-сладкая ягода",             Category = "Еда",       DifficultyRaw = "Easy" },
                new WordItem { Word = "самокат",  Hint = "транспорт для детей и взрослых",         Category = "Предметы",  DifficultyRaw = "Easy" },
                new WordItem { Word = "енот",     Hint = "животное с «маской» на морде",           Category = "Животные",  DifficultyRaw = "Easy" },
                new WordItem { Word = "лягушка",  Hint = "зелёное земноводное",                    Category = "Животные",  DifficultyRaw = "Easy" },
                new WordItem { Word = "кувшин",   Hint = "сосуд для воды или вина",                Category = "Предметы",  DifficultyRaw = "Easy" },

                // Normal (10)
                new WordItem { Word = "мангуст",  Hint = "зверёк, враг змей",                      Category = "Животные",  DifficultyRaw = "Normal" },
                new WordItem { Word = "василиск", Hint = "существо, которое может убить взглядом", Category = "Мифология", DifficultyRaw = "Normal" },
                new WordItem { Word = "гранат",   Hint = "фрукт с множеством зёрен",               Category = "Еда",       DifficultyRaw = "Normal" },
                new WordItem { Word = "самурай",  Hint = "воин в Японии",                          Category = "Мифология", DifficultyRaw = "Normal" },
                new WordItem { Word = "фонарь",   Hint = "источает свет в темноте",                Category = "Предметы",  DifficultyRaw = "Normal" },
                new WordItem { Word = "зебра",    Hint = "полосатое животное из Африки",           Category = "Животные",  DifficultyRaw = "Normal" },
                new WordItem { Word = "хинкали",  Hint = "тесто с мясной начинкой",                Category = "Еда",       DifficultyRaw = "Normal" },
                new WordItem { Word = "пергамент",Hint = "материал для письма в древности",        Category = "Предметы",  DifficultyRaw = "Normal" },
                new WordItem { Word = "цербер",   Hint = "трёхголовый пёс Аида",                   Category = "Мифология", DifficultyRaw = "Normal" },
                new WordItem { Word = "кактус",   Hint = "растение пустыни с колючками",           Category = "Предметы",  DifficultyRaw = "Normal" },

                // Hard (10)
                new WordItem { Word = "хамелеон", Hint = "умеет менять цвет кожи",                 Category = "Животные",  DifficultyRaw = "Hard" },
                new WordItem { Word = "амброзия", Hint = "пища богов в мифах",                     Category = "Мифология", DifficultyRaw = "Hard" },
                new WordItem { Word = "зазеркалье", Hint = "мир по ту сторону отражения",          Category = "Мифология", DifficultyRaw = "Hard" },
                new WordItem { Word = "тмин",     Hint = "пряность с характерным ароматом",        Category = "Еда",       DifficultyRaw = "Hard" },
                new WordItem { Word = "луковица", Hint = "основа для борща и супа",                Category = "Еда",       DifficultyRaw = "Hard" },
                new WordItem { Word = "астролябия", Hint = "древний астрономический прибор",       Category = "Предметы",  DifficultyRaw = "Hard" },
                new WordItem { Word = "фолиант",  Hint = "очень большая книга",                    Category = "Предметы",  DifficultyRaw = "Hard" },
                new WordItem { Word = "гарпия",   Hint = "женщина-птица из мифов",                 Category = "Мифология", DifficultyRaw = "Hard" },
                new WordItem { Word = "мангустин",Hint = "экзотический фиолетовый фрукт",          Category = "Еда",       DifficultyRaw = "Hard" },
                new WordItem { Word = "аксолотль",Hint = "редкое мексиканское земноводное",        Category = "Животные",  DifficultyRaw = "Hard" }
            ]
        };
    }
}
