using System;
using System.Text;
using System.Text.RegularExpressions;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Extensions.String
{
    /// <summary>
    /// Расширения для строк связанные с конвертирование кодировок.
    /// </summary>
    public static class StringEncodingExtensions
    {
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
                // This character is ASCII
                if (c <= 127)
                {
                    sb.Append(c);
                    continue;
                }

                sb.Append("\\u");
                sb.Append(((int)c).ToString("x4"));
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
                ? Regex.Replace(value, @"\\u([\dA-Fa-f]{4})", v => ((char) Convert.ToInt32(v.Groups[1].Value, 16)).ToString())
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
        
    }
}