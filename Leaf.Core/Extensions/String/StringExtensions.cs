using System;
using System.Collections.Generic;
using System.Globalization;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Extensions.String
{
    /// <summary>
    /// Расширения при работе со строками.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Проверяет наличие слова в строке, аналогично <see cref="string.Contains(string)"/>, но без учета реестра и региональных стандартов.
        /// </summary>
        /// <param name="str">Строка для поиска слова</param>
        /// <param name="value">Слово которое должно содержаться в строке</param>
        /// <returns>Вернет истину если значение было найдено в строке</returns>
        public static bool ContainsIgnoreCase(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        /// <summary>
        /// Проверяет наличие строки в массиве или списке, с правил сравнения строк.
        /// </summary>
        /// <param name="self">Массив или список где следует искать значение</param>
        /// <param name="value">Значение для поиска</param>
        /// <param name="comparison">Правило сравнения строк</param>
        /// <returns>Вернет истину если элемент был найден</returns>
        public static bool Contains(this IReadOnlyList<string> self, string value, StringComparison comparison = StringComparison.Ordinal)
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
        /// Преобразовывает первую букву в верхний реестр.
        /// </summary>
        /// <param name="s">Строка которая должна быть с первой заглавной буквой</param>
        /// <param name="useToLower">Следует ли преобразовавать все символы кроме первого в нижний реестр</param>
        /// <returns>Строка с первой заглавной буквой.</returns>
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
        /// Получает из JSON значение нужного ключа.
        /// </summary>
        /// <param name="json">JSON строка</param>
        /// <param name="key">Ключ</param>
        /// <param name="endsWith">Окончание значения (по умолчанию - окончание строка)</param>
        /// <returns>Вернет значение JSON ключа</returns>
        public static string GetJsonValue(this string json, string key, string endsWith = ",\"")
        {
            return endsWith != "\"" 
                ? json.Between($"\"{key}\":", endsWith)?.Trim('"', '\r', '\n', '\t') 
                : json.Between($"\"{key}\":\"", "\"");
        }

        /// <inheritdoc cref="GetJsonValue(string, string, string)"/>
        /// <summary>
        /// Получает из JSON значение нужного ключа и бросает исключение <exception cref="StringBetweenException" /> если ключ не был найден.
        /// </summary>
        /// <exception cref="StringBetweenException">Бросает если значение ключа не было найдено</exception>
        public static string GetJsonValueEx(this string json, string key, string ending = ",\"")
        {
            return GetJsonValue(json, key, ending)
                ?? throw new StringBetweenException($"Не найдено значение JSON ключа \"{key}\". Ответ: {json}");
        }


        /// <summary>
        /// Конвертирует HEX строку в оригинальный набор байт
        /// </summary>
        /// <param name="hexString">Строка сданными в виде HEX</param>
        /// <returns>Оригинальный набор байт</returns>
        public static byte[] HexStringToBytes(this string hexString)
        {
            int numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

            return bytes;
        }

        /// <summary>
        /// Экранирует " символы и символы юникода в JSON.
        /// </summary>
        /// <param name="jsonData">JSON данные</param>
        /// <param name="escapeUnicode">Следует ли экранировать символы юникода</param>
        /// <returns>Вернет экранированные данные.</returns>
        public static string EscapeJsonData(this string jsonData, bool escapeUnicode)
        {
            string result = jsonData
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");

            if (escapeUnicode)
                result = result.EncodeJsonUnicode();

            return result;
        }

        /// <summary>
        /// Возращает тип форматирование числа с разделением тысяч.
        /// </summary>
        public static NumberFormatInfo ThousandNumberFormatInfo
        {
            get {
                // ReSharper disable once InvertIf
                if (_thousandNumberFormatInfo == null)
                {
                    _thousandNumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                    _thousandNumberFormatInfo.NumberGroupSeparator = " ";
                }

                return _thousandNumberFormatInfo;
            }
        }
        private static NumberFormatInfo _thousandNumberFormatInfo;

    }
}