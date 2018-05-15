using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Leaf.Core.Runtime.Serialization;
using Leaf.Core.Threading;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Фабрика потокобезопасных коллекций.
    /// Используйте её когда требуется инициализировать потокобезопасную коллекцию из файла.
    /// </summary>
    public static class ConcurrentFactory
    {
        #region # Public Synchronous Methods

        #region ## Generic

        /// <summary>
        /// Создаёт потокобезопасный список объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="ui">Потокобезопасный интерфейс, нужен для ведения лога в случае ошибки десериализации</param>
        /// <param name="includeComments">Если true, то строки с коментариями тоже будут включены в выборку для десериалиализации.</param>
        /// <param name="trim">Очищать начало и конец строк от отступов и пробелов.</param>
        /// <returns>Возвращает новый потокобезопасный список объектов</returns>
        public static ConcurrentBag<T> BagFromFile<T>(string filePath, ThreadSafeUI ui = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            var result = new ConcurrentBag<T>();
            ReadAndDeserialize(result, filePath, ui, includeComments, trim);
            return result;
        }

        /// <summary>
        /// Создаёт потокобезопасную очередь объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="ui">Потокобезопасный интерфейс, нужен для ведения лога в случае ошибки десериализации</param>
        /// <param name="includeComments">Если true, то строки с коментариями тоже будут включены в выборку для десериалиализации.</param>
        /// <param name="trim">Очищать начало и конец строк от отступов и пробелов.</param>
        /// <returns>Возвращает новую потокобезопасную очередь объектов</returns>
        public static ConcurrentQueue<T> QueueFromFile<T>(string filePath, ThreadSafeUI ui = null, 
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            var result = new ConcurrentQueue<T>();
            ReadAndDeserialize(result, filePath, ui, includeComments, trim);      
            return result;
        }

        #endregion

        #region ## String

        /// <inheritdoc cref="BagFromFile{T}"/>
        /// <summary>
        /// Создаёт потокобезопасный список строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <returns>Возвращает новый потокобезопасный список строк</returns>
        public static ConcurrentBag<string> BagFromFile(string filePath, bool includeComments = false, bool trim = true)
        {
            var result = new ConcurrentBag<string>();
            ReadAndAppend(result, filePath, includeComments, trim);
            return result;
        }

        /// <inheritdoc cref="QueueFromFile{T}"/>
        /// <summary>
        /// Создаёт потокобезопасную очередь строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        /// <returns>Возвращает новую потокобезопасную очередь строк</returns>
        public static ConcurrentQueue<string> QueueFromFile(string filePath, bool includeComments = false, bool trim = true)
        {
            var result = new ConcurrentQueue<string>();
            ReadAndAppend(result, filePath, includeComments, trim);
            return result;
        }

        #endregion

        #endregion

        #region # Public Asynchronous Methods

        #region ## Generic

        /// <inheritdoc cref="BagFromFile{T}"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасный список объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>        
        public static async Task<ConcurrentBag<T>> BagFromFileAsync<T>(string filePath, ThreadSafeUI ui = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            return await Task.Run(() => BagFromFile<T>(filePath, ui, includeComments, trim));
        }

        /// <inheritdoc cref="QueueFromFile{T}"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасную очередь объектов из текстового файла. Десериализация проводится построчно.
        /// </summary>
        public static async Task<ConcurrentQueue<T>> QueueFromFileAsync<T>(string filePath, ThreadSafeUI ui = null,
            bool includeComments = false, bool trim = true)
            where T : IStringSerializeable, new()
        {
            return await Task.Run(() => QueueFromFile<T>(filePath, ui, includeComments, trim));
        }

        #endregion

        #region ## String

        /// <inheritdoc cref="BagFromFile"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасный список строк из текстового файла. Десериализация проводится построчно.
        /// </summary>
        public static async Task<ConcurrentBag<string>> BagFromFileAsync(string filePath, 
            bool includeComments = false, bool trim = true)
        {
            return await Task.Run(() => BagFromFile(filePath, includeComments, trim));
        }

        /// <inheritdoc cref="QueueFromFile"/>
        /// <summary>
        /// Асинхронно создаёт потокобезопасную очередь строк из текстового файла. Десериализация проводится построчно.
        /// </summary>        
        public static async Task<ConcurrentQueue<string>> QueueFromFileAsync(string filePath,
            bool includeComments = false, bool trim = true)
        {
            return await Task.Run(() => QueueFromFile(filePath, includeComments, trim));
        }

        #endregion

        #endregion

        #region Private helpers
        private delegate void LineProcessor(ulong lineNumber, string line);

        private static void ReadFileLineByLine(string filePath, bool includeComments, bool trim, LineProcessor lineProcessor)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
                return;
            }

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

        private static void ReadAndDeserialize<T>(IProducerConsumerCollection<T> collection, string filePath, ThreadSafeUI ui,
            bool includeComments, bool trim)
            where T : IStringSerializeable, new()
        {
            ReadFileLineByLine(filePath, includeComments, trim, (lineNumber, line) => {
                // Десериализуем объект из строки
                var item = new T();
                if (item.DeserializeFromString(line)) {
                    
                    if (!collection.TryAdd(item))
                        throw new IOException("Unable to ReadAndAppend to ConcurrentCollection.");
                }
                else
                    ui?.Log($"Пропускаю, неверная запись объекта {typeof(T).Name}. Строка #{lineNumber}: {line}");
            });
        }

        private static void ReadAndAppend(IProducerConsumerCollection<string> collection, string filePath,
            bool includeComments, bool trim)
        {
            ReadFileLineByLine(filePath, includeComments, trim, (lineNumber, line) => {

                if (!collection.TryAdd(line))
                    throw new IOException("Unable to ReadAndAppend to ConcurrentCollection.");
            });
        }
        #endregion
    }
}
