﻿using System;
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
        public const string HttpProto = "http://";
        public const string HttpsProto = "https://";

        /// <summary>
        /// Проверяет строку, является ли она ссылкой с протоколом http:// или https://.
        /// </summary>
        /// <param name="self">Ссылка</param>
        /// <param name="trim">Следует ли отсечь пробелы в начале и конце ссылки перед проверкой</param>
        /// <returns>Вернет <see langword="true"/> если строка оказалось ссылкой начинающийся на http:// или https://.</returns>
        public static bool IsWebLink(this string self, bool trim = false)
        {
            string link = self;
            if (trim)
                link = link.Trim();

            return link.StartsWith(HttpProto) || link.StartsWith(HttpsProto);
        }

        /// <summary>
        /// Проверяет строку на равенство пустой строке и возвращает null если равенство соблюдено.
        /// Используется для цепочных ? вызовов.
        /// </summary>
        /// <returns>Вернет <see langword="null"/> если строка равна <see cref="string.Empty"/>.</returns>
        public static string NullOnEmpty(this string self) => self == string.Empty ? null : self;

        /// <inheritdoc cref="string.IsNullOrEmpty"/>
        /// <summary>
        /// Расширение для метода <see cref="string.IsNullOrEmpty"/>.
        /// </summary>
        /// <param name="self">Строка</param>
        public static bool NullOrEmpty(this string self) => string.IsNullOrEmpty(self);

        /// <summary>
        /// Инвертированный вызов <see cref="string.IsNullOrEmpty"/>.
        /// </summary>
        /// <returns>Вернет <see langword="true"/> если строка не является пустой или <see langword="null"/>.</returns>
        public static bool NotNullNotEmpty(this string self) => !string.IsNullOrEmpty(self);

        /// <summary>
        /// Проверка строки на полезный контент.
        /// </summary>
        /// <param name="self">Строка</param>
        /// <returns>Вернет <see langword="true" /> если строка не является <see langword="null"/>, пустой строкой и пробелами.</returns>
        public static bool HasContent(this string self) => !string.IsNullOrWhiteSpace(self);

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
        /// <param name="useToLower">Следует ли преобразовывать все символы кроме первого в нижний реестр</param>
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
        /// Возвращает тип форматирование числа с разделением тысяч.
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