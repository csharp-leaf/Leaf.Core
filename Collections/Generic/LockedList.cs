using System;
//using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Leaf.Core.Collections.Generic
{
    // TODO: use object lockers. Different for read and write.
    // TODO: concurrent alternatives http://www.c-sharpcorner.com/article/thread-safe-concurrent-collection-in-C-Sharp/

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

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            lock (Storage)
            {
                return ListStorage.Where(predicate);
            }
        }

        public IEnumerable<T> Where(Func<T, int, bool> predicate)
        {
            lock (Storage)
            {
                return ListStorage.Where(predicate);
            }
        }

        public T First(Func<T, bool> predicate)
        {
            lock (Storage)
            {
                return ListStorage.First(predicate);
            }
        }

        public ListStorage<T> GetUnsafeStorage()
        {
            return ListStorage;
            // TODO: this
            // var test = new ConcurrentBag<string>();
        }
    }
}
