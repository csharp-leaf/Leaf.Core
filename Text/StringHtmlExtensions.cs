using System;
using System.Collections.Generic;

namespace Leaf.Core.Text
{
    public static class StringHtmlExtensions
    {
        private static bool HasSubstring(this string self, string left, string right, 
            out string substring,
            out int beginSubstringIndex,
            int startIndex = 0, StringComparison comparsion = StringComparison.Ordinal)
        {
            substring = null;
            beginSubstringIndex = -1;

            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) ||
                startIndex < 0 || startIndex >= self.Length)
                return false;

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = self.IndexOf(left, startIndex, comparsion);
            if (leftPosBegin == -1)
                return false;

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;

            // Ищем начало позиции правой строки.
            int rightPos = self.IndexOf(right, leftPosEnd, comparsion);
            if (rightPos == -1)
                return false;

            substring = self.Substring(leftPosEnd, rightPos - leftPosEnd);
            beginSubstringIndex = leftPosBegin;

            return true;
        }

        private static int GetInnerHtmlByAttribute(string self, string attribute, string value, ref string result,
            int startIndex = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            // Пока находятся классовые элементы
            while (self.HasSubstring(attribute + "=\"", "\"", out string classValue,
                out int index, startIndex, comparison))
            {
                // Чтобы не зацикливать поиск
                startIndex += attribute.Length + value.Length + 1; // class="val"

                // Убрираем мусор: пробелы, отступы
                classValue = classValue.Trim();

                // Пропускаем пустой класс атрибут
                if (value == string.Empty)
                    continue;

                // Проверяем наличие нужного класса
                var classes = classValue.Split(new[] { '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!classes.Contains(value, comparison))
                    continue;

                // Находим начало открывающего тега
                int openTagIndex = self.LastIndexOf("<", index - 1, StringComparison.Ordinal);
                if (openTagIndex == -1)
                    continue;

                // Определяем название тега
                string tagName = self.Substring(openTagIndex + 1, index - openTagIndex - 2);
                tagName = tagName.Split(new [] {' ', '\t', '\n'}, 2)[0];
                if (string.IsNullOrEmpty(tagName))
                    continue;
                int tagNameLength = tagName.Length;

                // Определяем конец открывающего тега
                int contentIndex = self.IndexOf(">", index + attribute.Length + value.Length + 1, // + 1 потому что "
                    StringComparison.InvariantCulture);
                if (contentIndex == -1)
                    continue;

                // Передвигаем индекс на начало контента, чтобы исключить символ тега из результата
                ++contentIndex;

                int tagCounter = 0;
                int maxSourceIndex = self.Length - 1;

                // Посимвольно ведем подсчет открывающих и закрывающих тегов
                // Remark: i < maxSourceIndex, а не <= потому что используем i + 1 символ
                for (int i = contentIndex; i < maxSourceIndex; i++)
                {
                    char c = self[i];

                    // Если найден символ тега - определяем что это за тег
                    if (c != '<')
                        continue;

                    // Получаем индекс следующего символа, который будет определять тег
                    int n = i + 1;
                    bool isClosingTag = self[n] == '/';
                    int factor;

                    if (isClosingTag)
                    {
                        // Т.к. имеем закрывающий тег, берем следующий символ для определения совпадения
                        ++n;
                        // Будем вычитать из tagCounter, т.к. нашли закрывающий тег
                        factor = -1;
                    }
                    else
                    {
                        // Будем добавлять к tagCounter, т.к. нашли новый открывающийся тег
                        factor = 1;
                    }

                    // Если вышли за пределы исходной длинны строки - выходим из цикла
                    if (n + tagNameLength > maxSourceIndex)
                        break;

                    // Hack: оптимизация по первому символу
                    // Если первый символ тега совпадает с тем именем тега что мы ищем,
                    // а после и совпадает полностью, увеличиваем или уменьшаем подсчет тегов.
                    if (self[n] != tagName[0] || tagName != self.Substring(n, tagNameLength))
                        continue;

                    tagCounter += factor;

                    // Если число закрывающих тегов превысело открывающих - значит нашли необходимый
                    if (tagCounter != -1)
                        continue;

                    result = self.Substring(contentIndex, i - contentIndex);
                    int endResultIndex = index + result.Length;
                    return endResultIndex >= self.Length ? -1 : endResultIndex;
                }
            }

            return -1;
        }

        private static int GetInnerHtmlByClass(string self, string className, ref string result,
            int startIndex = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            return GetInnerHtmlByAttribute(self, "class", className, ref result, startIndex, comparison);
        }

        /// <summary>
        /// Выбирает внутрений HTML код первого найденного элемента с соответствующим классом.
        /// </summary>
        /// <param name="self">Исходный HTML</param>
        /// <param name="className">Имя класса который нужно искать</param>
        /// <param name="startIndex">Начальный индекс поиска</param>
        /// <param name="comparison">Способ сравнения строк имен классов</param>
        /// <returns>Вернет внутренний HTML код элемента</returns>
        public static string InnerHtmlByClass(this string self, string className, int startIndex = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            string result = string.Empty;

            GetInnerHtmlByClass(self, className, ref result, startIndex, comparison);

            return result;
        }
        /*
         * not tested trash
        public static string InnerHtmlByTag(this string self, string tag, int startIndex = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            string result = string.Empty;

            int lastIndex = startIndex;

            while (self.HasSubstring("<" + tag, "</" + tag, out string rawInner, out int beginIndex, lastIndex))
            {
                lastIndex = beginIndex + rawInner.Length;

                // Если тег попал под шаблон, начала строки, но имеет продолжение к названию тега
                // div тег != diver тег, смысл именно в этом
                // Тут может быть только табуляция или >
                char c = rawInner[0];
                if (char.IsLetterOrDigit(c))
                    continue;

                int cropStartIndex = rawInner.IndexOf(">", StringComparison.Ordinal);
                if (cropStartIndex == -1)
                    continue;

                result = rawInner.Substring(cropStartIndex + 1, rawInner.Length - 1 - cropStartIndex);
                break;
            }

            return result;
        }*/

        public static string InnerHtmlByAttribute(this string self, string attribute, string value, int startIndex = 0,
            StringComparison comparison = StringComparison.Ordinal)
        {
            string result = string.Empty;

            GetInnerHtmlByAttribute(self, attribute, value, ref result, startIndex, comparison);

            return result;
        }


        public static string[] InnerHtmlByClassAll(this string self, string className, int startIndex = 0,
            bool trim = false,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int currentIndex = startIndex;

            string item = string.Empty;

            var result = new List<string>();
            while ((currentIndex = GetInnerHtmlByClass(self, className, ref item, currentIndex, comparison)) != -1)
            {
                if (trim)
                    item = item.Trim();

                result.Add(item);
            }

            return result.ToArray();
        }
    }
}
