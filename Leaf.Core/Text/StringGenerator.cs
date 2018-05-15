using System;
using System.Text;
using Leaf.Core.Extensions.String;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Text
{
    /// <summary>
    /// Потокобезопасный генератор логинов и паролей.
    /// </summary>
    public static class StringGenerator
    {
        [ThreadStatic] private static Random _rand;
        private static Random Rand => _rand ?? (_rand = new Random());

        // Прилагательные / глаголы
        private static readonly string[] Adjectives = {
            "active", "actual", "alive", "alone", "angry", "any", "aware", "basic", "best", "big", "boring", "brave", "brief", "bright", "brown", "busy", "cheap", "clean", "cold", "common", "cool", "crazy", "cute", "dark", "dear", "deep", "direct", "dirty", "double", "drunk", "dry", "easy", "empty", "entire", "equal", "every", "exact", "expert", "extra", "fair", "famous", "far", "fast", "final", "fine", "first", "fit", "flat", "formal", "free", "fresh", "front", "full", "fun", "funny", "glad", "glass", "global", "gold", "good", "grand", "great", "green", "gross", "guilty", "happy", "hard", "head", "heavy", "high", "honest", "hot", "huge", "hungry", "ideal", "inner", "just", "kind", "known", "large", "last", "late", "least", "left", "legal", "little", "living", "lonely", "long", "lost", "loud", "low", "lucky", "mad", "main", "major", "medium", "mental", "middle", "minor", "more", "most", "much", "narrow", "nasty", "native", "nearby", "neat", "new", "next", "nice", "normal", "north", "novel", "other", "over", "past", "plenty", "pretty", "prior", "public", "pure", "purple", "quick", "quiet", "rare", "raw", "real", "recent", "red", "remote", "rich", "right", "round", "royal", "sad", "scared", "secret", "secure", "senior", "sharp", "short", "signal", "silver", "simple", "single", "slight", "slow", "small", "smart", "smooth", "soft", "solid", "south", "stock", "strict", "strong", "such", "super", "sweet", "thin", "tight", "tiny", "top", "unfair", "unique", "united", "usual", "visual", "warm", "waste", "weird", "white", "wide", "wild", "wise", "wooden", "worth", "wrong", "yellow"
        };

        // Существительные
        private static readonly string[] Nouns = {
            "access", "act", "action", "actor", "affair", "agency", "air", "answer", "apple", "area", "army", "art", "aspect", "basket", "bird", "blood", "board", "boat", "book", "boss", "box", "breath", "card", "case", "cash", "cause", "chance", "cheek", "child", "church", "coast", "coffee", "cookie", "data", "dealer", "demand", "depth", "design", "desk", "device", "dog", "drawer", "driver", "end", "energy", "entry", "error", "estate", "event", "extent", "fact", "farmer", "figure", "fire", "fish", "focus", "force", "form", "frame", "future", "game", "garden", "gate", "girl", "goal", "group", "hat", "heart", "house", "idea", "image", "insect", "item", "job", "key", "king", "lab", "lady", "lake", "law", "leader", "level", "life", "line", "list", "loss", "love", "man", "math", "media", "member", "memory", "mind", "model", "moment", "money", "mouse", "movie", "nation", "night", "note", "number", "order", "page", "paper", "people", "person", "phone", "photo", "pizza", "player", "point", "power", "queen", "reason", "recipe", "record", "risk", "river", "road", "rock", "rule", "sample", "sense", "sir", "song", "sound", "source", "sport", "store", "story", "studio", "style", "tale", "term", "theory", "thing", "time", "trade", "truth", "unit", "user", "virus", "war", "way", "web", "winner", "woman", "world", "writer"
        };

        /// <summary>
        /// Генерирует случайную строку.
        /// </summary>
        /// <param name="wordUpperFirst">Каждое слово должно быть с большой буквы</param>
        /// <param name="minDigits">Минимальное число цифр в конце строки</param>
        /// <param name="maxDigits">Минимальное число цифр в конце строки</param>
        /// <remarks>
        /// Если указать в качестве аргументов нули - цифры не будут добавлены.
        /// </remarks>
        /// <param name="wordSeparator">Разделитель между словами</param>
        public static string Random(bool wordUpperFirst = false, int minDigits = 0, int maxDigits = 0, string wordSeparator = null)
        {
            var result = new StringBuilder();

            // добавляем прилагательное или глагол
            string adj = Adjectives[Rand.Next(Adjectives.Length - 1)];
            if (wordUpperFirst)
                adj = adj.ToUpperFirst(false); // (оптимизационный выхов) только 1я заглавная, остальные буквы без изменений
            result.Append(adj);

            // добавляем разделитель если он установлен
            if (wordSeparator != null)
                result.Append(wordSeparator);

            // добавляем существительное
            string noun = Nouns[Rand.Next(Nouns.Length - 1)];
            if (wordUpperFirst)
                noun = noun.ToUpperFirst(false); // (оптимизационный выхов) только 1я заглавная, остальные буквы без изменений
            result.Append(noun);

            // добавляем число в конце если нужно
            if (minDigits != 0 && maxDigits != 0)
                result.AppendRandomNumbers(minDigits, maxDigits);

            // возвращаем результат
            return result.ToString();
        }

        /// <summary>
        /// Генерирует легко читаемый логин для ресурса. В конце логина добавляются от 1 до 3 случайных цифр.
        /// </summary>
        /// <param name="wordSeparator">Разделитель между словами</param>
        public static string RandomLogin(string wordSeparator = null)
        {
            return Random(false, 1, 3, wordSeparator);
        }

        /// <summary>
        /// Генерирует легко читаемый случайный пароль c двумя цифрами на конце.
        /// Каждое слово в пароле пишется с заглавной буквы, без разделителя.
        /// </summary>
        public static string RandomPassword()
        {
            return Random(true, 2, 2);
        }

        /// <summary>
        /// Генерирует легко читаемый случайный пароль c нужным количеством цифр на конце.
        /// Каждое слово в пароле пишется с заглавной буквы, без разделителя.
        /// </summary>
        /// <param name="minDigits">Минимальное число цифр в конце пароля</param>
        /// <param name="maxDigits">Минимальное число цифр в конце пароля</param>
        /// <remarks>
        /// Если указать в качестве аргументов нули - цифры не будут добавлены.
        /// </remarks>
        public static string RandomPassword(int minDigits, int maxDigits)
        {
            return Random(true, minDigits, maxDigits);
        }

        private static void AppendRandomNumbers(this StringBuilder sb, int minDigits, int maxDigits)
        {
            // проверка параметров
            if (minDigits > maxDigits || maxDigits < minDigits) // || minDigits == 0 || maxDigits == 0
                throw new ArgumentException("Неверно заданы количесво цифр для добавления в StringBuilder");

            // частная оптимизация
            if (minDigits == 1 && maxDigits == 1)
            {
                sb.Append(Rand.Next(0, 9));
                return;
            }

            // получаем предел в соответствии с максимальным количеством цифр max = 10 ^ maxDigits - 1;
            int max = 1;
            for (int i = 0; i < maxDigits; i++)
                max *= 10;

            // дополняем нулями минимальную длинну цифр
            string random = Rand.Next(0, max - 1).ToString();
            int randomLength = random.Length;

            while (randomLength < minDigits)
            {
                sb.Append('0');
                ++randomLength;
            }

            sb.Append(random);
        }
    }
}
