using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Потокобезопасная очередь материалов.
    /// </summary>
    public class MaterialsQueue<T> : MaterialsCollection<T>
    {
        protected readonly MaterialsStorageQueue<T> MaterialsStorageQueue = new MaterialsStorageQueue<T>();

        /// <summary>
        /// Создает новую очередь материалов.
        /// </summary>
        public MaterialsQueue()
        {
            MaterialsStorage = MaterialsStorageQueue;
        }

        /// <summary>
        /// Создает новую очередь материалов на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items">Элементы которые следует добавить в очередь</param>
        public MaterialsQueue(IEnumerable<T> items) : this() // Вызываем базовый конструктор сначала
        {
            foreach (var item in items)
                MaterialsStorageQueue.Enqueue(item);
        }
    }
}
