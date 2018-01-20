using System.IO;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections.FileBased
{
    public delegate void BeforeReadFileHandler();

    public abstract class FileMaterialsBase : MaterialsBase<string>
    {
        public event BeforeReadFileHandler BeforeReadFile;

        /// <summary>
        /// Имя файла из которого взята коллекция материалов.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Общее число элементов взятых из файла при инициализации.
        /// </summary>
        public int TotalCount { get; private set; } // всего записей при старте
        private readonly bool _includeComments;

        /// <summary>
        /// Создаёт коллекцию сторок из файла.
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="includeComments">Если true, то коментарии тоже будут включены в выборку</param>
        protected FileMaterialsBase(string fileName, bool includeComments = false)
        {
            FileName = fileName;
            _includeComments = includeComments;
        }

        /// <summary>
        /// Загрузка материалов из файла.
        /// </summary>
        /// <returns>Вернет истину если файл был успешно прочитан, а очередь заполнена материалами.</returns>
        public bool ReadFromFile()
        {
            lock (MaterialsStorage)
            {
                try
                {
                    if (!File.Exists(FileName))
                        return false;

                    if (MaterialsStorage.Count > 0)
                        MaterialsStorage.Clear();

                    BeforeReadFile?.Invoke();

                    using (var file = new StreamReader(FileName))
                    {
                        while (!file.EndOfStream)
                        {
                            string line = file.ReadLine();
                            if (string.IsNullOrWhiteSpace(line) || !_includeComments 
                                && (line.StartsWith("//") || line.StartsWith("#")))
                                continue;

                            if (!string.IsNullOrEmpty(line))
                                MaterialsStorage.AppendItem(line.Trim());
                        }

                        TotalCount = MaterialsStorage.Count;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
