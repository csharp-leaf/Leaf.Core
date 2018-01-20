using System.Text;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections.FileBased
{
    /// <summary>
    /// Потокобезопасная очередь строк-материалов, читаемая из файла.
    /// </summary>
    public class FileMaterialsQueue : FileMaterialsBase
    {
        private readonly MaterialsStorageQueue<string> _materialsStorage = new MaterialsStorageQueue<string>();

        /// <inheritdoc />
        /// <summary>
        /// Создаёт потокобезопасную очередь строк-материалов из файла.
        /// </summary>
        public FileMaterialsQueue(string fileName, bool includeComments = false) : base(fileName, includeComments)
        {
            MaterialsStorage = _materialsStorage;
            ReadFromFile();
        }       

        /// <summary>
        /// Поочередно извлекает все материалы из очереди и записывает в строку.
        /// </summary>
        /// <returns>Строка со всеми материалами. Каждый материал будет идти с новой строки.</returns>
        public string DeqeueAllToString()
        {
            if (MaterialsStorage == null)
                return string.Empty;

            var sb = new StringBuilder();

            lock (MaterialsStorage)
            {
                int materialsCount = MaterialsStorage.Count;

                for (int i = 0; i < materialsCount; i++)
                {
                    string material = MaterialsStorage.GetNext();
                    if (!string.IsNullOrEmpty(material))
                        sb.AppendLine(material.Trim());
                }
            }

            return sb.ToString();
        }
    }
}
