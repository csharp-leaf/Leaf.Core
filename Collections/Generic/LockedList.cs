using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    /// <inheritdoc/>
    /// <summary>
    /// Потокобезопасный список.
    /// </summary>
    public class LockedList<T> : LockedCollection<T>
    {
        protected readonly ListStorage<T> ListStorage = new ListStorage<T>();
        
        /// <summary>
        /// Изначальное число элементов. Назначение: подсчет прогресса.
        /// Значение задается на фабриках, например после чтения коллекции из файла.
        /// </summary>
        public int StartCount { get; set; }

        /// <summary>
        /// Создает новый потокобезопасный список.
        /// </summary>
        public LockedList()
        {
            Storage = ListStorage;
        }

        /// <summary>
        /// Создает новый потокобезопасный список на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items">Элементы которые следует добавить в список</param>
        public LockedList(IEnumerable<T> items) : this() // Вызываем базовый конструктор сначала
        {
            foreach (var item in items)
                ListStorage.Add(item);
        }

        /// <summary>
        /// Возвращает следующий случайный элемент из списка.
        /// </summary>
        public T GetNextRandom()
        {
            if (Storage == null)
                return default(T);

            lock (Storage)
                return ListStorage.GetNextRandom();
        }

        /// <summary>
        /// Устанавливает тип перечисления элементов списка.
        /// </summary>
        public ListIteration Iteration {
            get {
                lock (Storage)
                    return ListStorage.Iteration;
            }
            set {
                lock (Storage)
                    ListStorage.Iteration = value;
            }
        }

        /// <summary>
        /// Сбрасывает указатель текущего элемента списка на первый (нулевой индекс).
        /// </summary>
        public void ResetPointer()
        {
            lock (ListStorage)
                ListStorage.ResetPointer();
        }

        /// <summary>
        /// Проверяет существование элемента в списке.
        /// </summary>
        public bool Contains(T item)
        {
            if (Storage == null)
                return false;

            lock (Storage)
                return ListStorage.Contains(item);
        }

        /// <summary>
        /// Удаляет элемент из списка.
        /// </summary>
        /// <param name="item">Элемент</param>
        /// <returns>Возвращает истину, если элемент был найден и удалён.</returns>
        public bool Remove(T item)
        {
            if (Storage == null)
                return false;

            lock (Storage)
                return ListStorage.Remove(item);
        }
    }
}
