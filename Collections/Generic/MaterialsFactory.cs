using System.IO;
using System.Threading.Tasks;
using Leaf.Core.Threading;

namespace Leaf.Core.Collections.Generic
{
    public static class MaterialsFactory
    {
        #region # Public Synchronous Methods

        #region ## Generic

        /// <summary>
        /// Создаёт потокобезопасный список объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="log">Делегат для вывода сообщений об ошибках десериализации объектов</param>
        /// <param name="includeComments">Если true, то строки с коментариями тоже будут включены в выборку для десериалиализации.</param>
        /// <param name="trim">Очищать начало и конец строк от отступов и пробелов.</param>
        /// <returns>Новый список материалов</returns>
        public static MaterialsList<T> ListFromFile<T>(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            var result = new MaterialsList<T>();
            ReadAndDeserialize(result, filePath, log, includeComments, trim);
            return result;
        }

        /// <summary>
        /// Создаёт потокобезопасную очередь объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="log">Делегат для вывода сообщений об ошибках десериализации объектов</param>
        /// <param name="includeComments">Если true, то строки с коментариями тоже будут включены в выборку для десериалиализации.</param>
        /// <param name="trim">Очищать начало и конец строк от отступов и пробелов.</param>
        /// <returns>Новая очередь материалов</returns>
        public static MaterialsQueue<T> QueueFromFile<T>(string filePath, DFormLog log = null, 
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            var result = new MaterialsQueue<T>();
            ReadAndDeserialize(result, filePath, log, includeComments, trim);      
            return result;
        }

        #endregion

        #region ## String

        /// <inheritdoc cref="ListFromFile{T}"/>
        /// <summary>
        /// Создаёт потокобезопасный список строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <returns>Новый список строковых материалов</returns>
        public static MaterialsList ListFromFile(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
        {
            var result = new MaterialsList();
            ReadAndAppend(result, filePath, log, includeComments, trim);
            return result;
        }

        /// <inheritdoc cref="QueueFromFile{T}"/>
        /// <summary>
        /// Создаёт потокобезопасную очередь строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <returns>Новая очередь строковых материалов</returns>
        public static MaterialsQueue QueueFromFile(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
        {
            var result = new MaterialsQueue();
            ReadAndAppend(result, filePath, log, includeComments, trim);
            return result;
        }

        #endregion

        #endregion

        #region # Public Asynchronous Methods

        #region ## Generic

        /// <inheritdoc cref="ListFromFile{T}"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасный список объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>        
        public static async Task<MaterialsList<T>> ListFromFileAsync<T>(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            return await Task.Run(() => ListFromFile<T>(filePath, log, includeComments, trim));
        }

        /// <inheritdoc cref="QueueFromFile{T}"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасную очередь объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        public static async Task<MaterialsQueue<T>> QueueFromFileAsync<T>(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            return await Task.Run(() => QueueFromFile<T>(filePath, log, includeComments, trim));
        }

        #endregion

        #region ## String

        /// <inheritdoc cref="ListFromFile"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасный список строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        public static async Task<MaterialsList> ListFromFileAsync(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
        {
            return await Task.Run(() => ListFromFile(filePath, log, includeComments, trim));
        }

        /// <inheritdoc cref="QueueFromFile"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасную очередь строк из текстового файла. Десериализация проводится построчно.
        /// </summary>        
        public static async Task<MaterialsQueue> QueueFromFileAsync(string filePath, DFormLog log = null,
            bool includeComments = false, bool trim = true)
        {
            return await Task.Run(() => QueueFromFile(filePath, log, includeComments, trim));
        }

        #endregion

        #endregion

        #region Private helpers
        private delegate void LineProcessor(ulong lineNumber, string line);

        private static void ReadFileLineByLine(string filePath, bool includeComments, bool trim, LineProcessor lineProcessor)
        {
            using (var file = new StreamReader(filePath))
            {
                ulong lineNumber = 0;

                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    ++lineNumber;

                    // Пропускаем пустые строки и комментарии если требуется
                    if (string.IsNullOrWhiteSpace(line) ||
                        !includeComments && (line.StartsWith("//") || line.StartsWith("#")))
                        continue;

                    if (trim)
                        line = line.Trim();

                    lineProcessor(lineNumber, line);
                }
            }
        }

        private static void ReadAndDeserialize<T>(MaterialsCollection<T> collection, string filePath, DFormLog log,
            bool includeComments, bool trim)
            where T : IStringSerializeable, new()
        {
            ReadFileLineByLine(filePath, includeComments, trim, (lineNumber, line) => {
                // Десериализуем объект из строки
                var item = new T();
                if (item.DeserializeFromString(line))
                    collection.AppendItem(item);
                else
                    log?.Invoke($"Пропускаю, неверная запись объекта {nameof(T)}. Строка #{lineNumber}: {line}");
            });
        }

        private static void ReadAndAppend(MaterialsCollection<string> collection, string filePath, DFormLog log,
            bool includeComments, bool trim)
        {
            ReadFileLineByLine(filePath, includeComments, trim, (lineNumber, line) => {
                collection.AppendItem(line);
            });
        }
        #endregion
    }
}
