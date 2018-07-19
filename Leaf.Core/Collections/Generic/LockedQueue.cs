using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    /// <inheritdoc/>
    /// <summary>
    /// Потокобезопасная очередь.
    /// </summary>
    public class LockedQueue<T> : LockedCollection<T>
    {
        protected readonly QueueStorage<T> QueueStorage = new QueueStorage<T>();

        /// <summary>
        /// Создает новую потокобезопасную очередь.
        /// </summary>
        public LockedQueue()
        {
            Storage = QueueStorage;
        }

        /// <inheritdoc />
        /// <summary>
        /// Создает новую потокобезопасную очередь на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items">Элементы которые следует добавить в очередь</param>
        public LockedQueue(IEnumerable<T> items) : this() // Вызываем базовый конструктор сначала
        {
            foreach (var item in items)
                QueueStorage.Enqueue(item);
        }
    }
}
