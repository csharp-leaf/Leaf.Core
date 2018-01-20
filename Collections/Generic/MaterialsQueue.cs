using System.Collections.Generic;
using System.Text;

namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Потокобезопасная очередь материалов.
    /// </summary>
    public class MaterialsQueue<T> : MaterialsBase<T>
    {
        protected readonly MaterialsStorageQueue<T> MaterialsStorageQueue = new MaterialsStorageQueue<T>();

        /// <summary>
        /// Создает новый список материалов или на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items"></param>
        public MaterialsQueue(IEnumerable<T> items = null)
        {
            MaterialsStorage = MaterialsStorageQueue;

            if (items == null)
                return;

            foreach (var item in items)
                MaterialsStorageQueue.Enqueue(item);
        }

    }

    /// <summary>
    /// Потокобезопасная очередь строк-материалов.
    /// </summary>
    public class MaterialsQueue : MaterialsQueue<string>
    {
        /// <inheritdoc />
        public MaterialsQueue(IEnumerable<string> items = null) : base(items)
        {
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
