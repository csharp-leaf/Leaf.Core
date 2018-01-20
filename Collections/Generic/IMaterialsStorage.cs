namespace Leaf.Core.Collections.Generic
{
    public interface IMaterialsStorage<T>
    {
        /// <summary>
        /// Число элементов в коллекции.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Очистить коллекцию.
        /// </summary>
        void Clear();

        /// <summary>
        /// Добавляет элемент в коллекцию.
        /// </summary>
        /// <param name="item"></param>
        void AppendItem(T item);

        /// <summary>
        /// Возвращает следующий элемент коллекции.
        /// </summary>
        /// <returns>Следующий элемент коллекции.</returns>
        T GetNext();
    }
}
