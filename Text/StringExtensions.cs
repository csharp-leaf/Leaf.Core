using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Leaf.Core.Text
{
    public static class StringExtensions
    {
        /// <summary>
        /// Проверяет содержит ли строка полезные данные, т.е. не пуста и не является сплошными пробелами или отступами.
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Возвращает истину если входная строка не пуста и не является сплошными пробелами или отступами.</returns>
        public static bool HasContent(this string str) => !string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Проверка, является ли строка пустой или null.
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Возвращает истину если входная строка является null или пустотой ("").</returns>
        public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// Проверка строки на наличие символов (включая пробелы и отступы).
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Возвращает истину если входная строка не является null или пустой строкой.</returns>
        public static bool IsNotEmpty(this string str) => !string.IsNullOrEmpty(str);

        /// <summary>
        /// Проверяет наличие слова в строке, аналогично <see cref="string.Contains"/>, но без учета реестра и региональных стандартов.
        /// </summary>
        /// <param name="str">Строка для поиска слова</param>
        /// <param name="value">Слово которое должно содержаться в строке</param>
        /// <returns>Вернет истину если значение было найдено в строке</returns>
        public static bool ContainsIgnoreCase(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        /// <summary>
        /// Вырезает несколько строк между двумя подстроками.
        /// </summary>
        /// <param name="str">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparsion">Метод сравнения строк</param>
        /// <param name="limit">Максимальное число подстрок для поиска</param>
        /// <returns>Возвращает массив подстрок которые попапают под шаблон</returns>
        public static string[] Subs(this string str, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal, int limit = 0)
        {
            #region Проверка параметров
            if (string.IsNullOrEmpty(str))
                return new string[0];

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(left));

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(right));

            if (startIndex < 0 || startIndex >= str.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Wrong start index");
            #endregion

            int currentStartIndex = startIndex;            
            int current = limit;
            var strings = new List<string>();

            while (true)
            {
                if (limit > 0)
                {
                    --current;
                    if (current < 0)
                        break;
                }

                // Ищем начало позиции левой подстроки.
                int leftPosBegin = str.IndexOf(left, currentStartIndex, comparsion);
                if (leftPosBegin == -1)
                    break;

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;
                // Ищем начало позиции правой строки.
                int rightPos = str.IndexOf(right, leftPosEnd, comparsion);
                if (rightPos == -1)
                    break;

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;
                strings.Add(str.Substring(leftPosEnd, length));
                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }
            return strings.ToArray();
        }

        /// <summary>
        /// Вырезает одну строку между двумя подстроками.
        /// </summary>
        /// <param name="str">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparsion">Метод сравнения строк</param>
        /// <returns>Возвращает строку между двумя подстроками</returns>
        public static string Sub(this string str, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) ||
                startIndex < 0 || startIndex >= str.Length)
                return string.Empty;

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = str.IndexOf(left, startIndex, comparsion);
            if (leftPosBegin == -1)
                return string.Empty;

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;
            // Ищем начало позиции правой строки.
            int rightPos = str.IndexOf(right, leftPosEnd, comparsion);

            return rightPos != -1 ? str.Substring(leftPosEnd, rightPos - leftPosEnd) : string.Empty;
        }

        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только в обратном направлении поиска
        /// </summary>
        /// <param name="str">Строка где следует искать подстроки</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса
        /// <remarks>Если указано значение -1, поиск ведется с самого конца строки</remarks>
        /// </param>
        /// <param name="comparsion">Метод сравнения строк</param>
        /// <returns>Возвращает строку между двумя подстроками</returns>
        public static string LastSub(this string str, string right, string left,
            int startIndex = -1, StringComparison comparsion = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) ||
                startIndex < -1 || startIndex >= str.Length)
                return string.Empty;

            if (startIndex == -1)
                startIndex = str.Length - 1;

            // Ищем начало позиции правой подстроки с конца строки
            int rightPosBegin = str.LastIndexOf(right, startIndex, comparsion);
            if (rightPosBegin == -1 || rightPosBegin == 0) // в обратном поиске имеет смысл проверять на 0
                return string.Empty;

            // Вычисляем начало позиции левой подстроки
            int leftPosBegin = str.LastIndexOf(left, rightPosBegin - 1, comparsion);
            // Если не найден левый конец или правая и левая подстрока склеены вместе - вернем пустую строку
            if (leftPosBegin == -1 || rightPosBegin - leftPosBegin == 1)
                return string.Empty;

            int leftPosEnd = leftPosBegin + 1;

            return str.Substring(leftPosEnd, rightPosBegin - leftPosEnd);
        }

        /// <summary>
        /// Преобразовывает первую букву в верхний реестр
        /// </summary>
        /// <param name="s">Строка которая должна быть с первой заглавной буквой</param>
        /// <param name="useToLower">Следует ли преобразовавать все символы кроме первого в нижний реестр.</param>
        /// <returns>Строка с первой заглавной буквой. Остальные символы не будут изменены если задан <see cref="useToLower"/> = false.</returns>
        public static string ToUpperFirst(this string s, bool useToLower = true)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char firstLetter = char.ToUpper(s[0]);
            string otherWords = s.Substring(1);
           
            if (useToLower)
                otherWords = otherWords.ToLower();

            return firstLetter + otherWords;
        }

        /// <summary>
        /// Проверяет наличие строки в массиве или списке, с правил сравнения строк.
        /// </summary>
        /// <param name="self">Массив или список где следует искать значение</param>
        /// <param name="value">Значение для поиска</param>
        /// <param name="comparison">Правило сравнения строк</param>
        /// <returns>Вернет истину если элемент был найден</returns>
        public static bool Contains(this IReadOnlyList<string> self, string value, StringComparison comparison)
        {
            // Faster manual code
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < self.Count; i++)
            {
                if (self[i].Equals(value, comparison))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Получает из JSON значение нужного ключа.
        /// </summary>
        /// <param name="json">JSON строка</param>
        /// <param name="key">ключ</param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string GetJsonValue(this string json, string key, string endsWith = ",\"")
        {
            return json.Sub("\"" + key + "\":", endsWith).Trim('"', '\r', '\n', '\t');
        }

        /// <summary>
        /// Преобразовывает юникод символы строки в вид \u0000.
        /// </summary>
        /// <param name="value">Строка с символами юникода</param>
        /// <returns>Закодированая строка Json с символами в виде \u0000</returns>
        public static string EncodeJsonUnicode(this string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    sb.Append("\\u" + ((int)c).ToString("x4"));
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Преобразовывает юникод символы вида \u0000 в строке в нормальный вид.
        /// </summary>
        /// <param name="value">Текст с символами вида \u0000</param>
        /// <returns>Возвращает раскодированный текст</returns>
        public static string DecodeJsonUnicode(this string value)
        {
            return !string.IsNullOrEmpty(value)
                ? Regex.Replace(value, @"\\u([\dA-Fa-f]{4})",
                    v => ((char) Convert.ToInt32(v.Groups[1].Value, 16)).ToString())
                : string.Empty;
        }
        
        /// <summary>
        /// Преобразовывает текст из кодировки Windows-1251 в UTF8
        /// </summary>
        /// <param name="source">Текст который нужно преобразовать</param>
        /// <returns>Текст в кодировке UTF8</returns>
        public static string Win1251ToUTF8(this string source)
        {
            var win1251 = Encoding.GetEncoding("Windows-1251");

            var utf8Bytes = win1251.GetBytes(source);
            var win1251Bytes = Encoding.Convert(Encoding.UTF8, win1251, utf8Bytes);

            return win1251.GetString(win1251Bytes);
        }

        /*
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static ulong MillisecondsFrom1970 => (ulong) (DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        */
    }
}