using System;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Extensions.String
{
    /// <summary>
    /// Расширения для строк: поиск между двумя подстроками.
    /// </summary>
    public static class StringBetweenExtensions
    {        
        #region Betweens - Вырезание нескольких строк

        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет пустой массив.
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparsion">Метод сравнения строк</param>
        /// <param name="limit">Максимальное число подстрок для поиска</param>
        /// <exception cref="ArgumentNullException">Возникает если один из параметров пустая строка или <keyword>null</keyword>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Возникает если начальный индекс превышает длинну строки.</exception>
        /// <returns>Возвращает массив подстрок которые попапают под шаблон или пустой массив если нет совпадений.</returns>
        public static string[] BetweensOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal, int limit = 0)
        {
            #region Проверка параметров
            if (string.IsNullOrEmpty(self))
                return new string[0];

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(left));

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(right));

            if (startIndex < 0 || startIndex >= self.Length)
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
                int leftPosBegin = self.IndexOf(left, currentStartIndex, comparsion);
                if (leftPosBegin == -1)
                    break;

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;
                // Ищем начало позиции правой строки.
                int rightPos = self.IndexOf(right, leftPosEnd, comparsion);
                if (rightPos == -1)
                    break;

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;
                strings.Add(self.Substring(leftPosEnd, length));
                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }
            
            return strings.ToArray();
        }


        /// <inheritdoc cref="BetweensOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.        
        /// </remarks>
        /// <example>
        /// str.Betweens("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="BetweensEx"/> - которая и так бросает исключение <see cref="StringBetweenException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <returns>Возвращает массив подстрок которые попапают под шаблон или <keyword>null</keyword>.</returns>
        public static string[] Betweens(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal, int limit = 0)
        {
            var result = BetweensOrEmpty(self, left, right, startIndex, comparsion, limit);
            return result.Length > 0 ? result : null;
        }


        /// <inheritdoc cref="BetweensOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="StringBetweenException"/>.
        /// </summary>
        /// <exception cref="StringBetweenException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает массив подстрок которые попапают под шаблон или бросает исключение <see cref="StringBetweenException"/> если совпадений не было найдено.</returns>
        public static string[] BetweensEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal, int limit = 0)
        {
            var result = BetweensOrEmpty(self, left, right, startIndex, comparsion, limit);
            if (result.Length == 0)
                throw new StringBetweenException($"StringBetweens not found. Left: \"{left}\". Right: \"{right}\".");

            return result;
        }

        #endregion



        #region Between - Вырезание одной подстроки. Прямой порядок (слева направо)

        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет <paramref name="notFoundValue"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.Between("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="BetweenEx"/> - которая и так бросает исключение <see cref="StringBetweenException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparsion">Метод сравнения строк</param>
        /// <param name="notFoundValue">Значение в случае если подстрока не найдена</param>
        /// <returns>Возвращает строку между двумя подстроками или <paramref name="notFoundValue"/> (по-умолчанию <keyword>null</keyword>).</returns>
        public static string Between(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal, string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) ||
                startIndex < 0 || startIndex >= self.Length)
                return notFoundValue;

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = self.IndexOf(left, startIndex, comparsion);
            if (leftPosBegin == -1)
                return notFoundValue;

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;
            // Ищем начало позиции правой строки.
            int rightPos = self.IndexOf(right, leftPosEnd, comparsion);

            return rightPos != -1 ? self.Substring(leftPosEnd, rightPos - leftPosEnd) : notFoundValue;
        }


        /// <inheritdoc cref="Between"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет пустую строку.
        /// </summary>
        /// <returns>Возвращает строку между двумя подстроками. Если совпадений нет, вернет пустую строку.</returns>
        public static string BetweenOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal)
        {
            return Between(self, left, right, startIndex, comparsion, string.Empty);
        }

        /// <inheritdoc cref="Between"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="StringBetweenException"/>.
        /// </summary>
        /// <exception cref="StringBetweenException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает строку между двумя подстроками или бросает исключение <see cref="StringBetweenException"/> если совпадений не было найдено.</returns>
        public static string BetweenEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal)
        {
            return Between(self, left, right, startIndex, comparsion)
                ?? throw new StringBetweenException($"StringBetween not found. Left: \"{left}\". Right: \"{right}\".");
        }


        #endregion


        #region Вырезание одной подстроки. Обратный порядок (справа налево)
        
        /// <inheritdoc cref="Between"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет <paramref name="notFoundValue"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.BetweenLast("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="BetweenLastEx"/> - которая и так бросает исключение <see cref="StringBetweenException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        public static string BetweenLast(this string self, string right, string left,
            int startIndex = -1, StringComparison comparsion = StringComparison.Ordinal,
            string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) ||
                startIndex < -1 || startIndex >= self.Length)
                return notFoundValue;

            if (startIndex == -1)
                startIndex = self.Length - 1;

            // Ищем начало позиции правой подстроки с конца строки
            int rightPosBegin = self.LastIndexOf(right, startIndex, comparsion);
            if (rightPosBegin == -1 || rightPosBegin == 0) // в обратном поиске имеет смысл проверять на 0
                return notFoundValue;

            // Вычисляем начало позиции левой подстроки
            int leftPosBegin = self.LastIndexOf(left, rightPosBegin - 1, comparsion);
            // Если не найден левый конец или правая и левая подстрока склеены вместе - вернем пустую строку
            if (leftPosBegin == -1 || rightPosBegin - leftPosBegin == 1)
                return notFoundValue;

            int leftPosEnd = leftPosBegin + left.Length;
            return self.Substring(leftPosEnd, rightPosBegin - leftPosEnd);
        }


        /// <inheritdoc cref="BetweenOrEmpty"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет пустую строку.
        /// </summary>
        public static string BetweenLastOrEmpty(this string self, string right, string left,
            int startIndex = -1, StringComparison comparsion = StringComparison.Ordinal)
        {
            return BetweenLast(self, right, left, startIndex, comparsion, string.Empty);
        }
        
        /// <inheritdoc cref="BetweenEx"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, будет брошено исключение <see cref="StringBetweenException"/>.
        /// </summary>
        public static string BetweenLastEx(this string self, string right, string left,
            int startIndex = -1, StringComparison comparsion = StringComparison.Ordinal)
        {
            return BetweenLast(self, right, left, startIndex, comparsion)
                ?? throw new StringBetweenException($"StringBetween not found. Right: \"{right}\". Left: \"{left}\".");
        }

        #endregion
    }
}