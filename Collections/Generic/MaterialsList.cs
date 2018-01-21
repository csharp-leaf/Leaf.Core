using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Потокобезопасный список материалов.
    /// </summary>
    public class MaterialsList<T> : MaterialsCollection<T>
    {
        protected readonly MaterialsStorageList<T> MaterialsStorageList = new MaterialsStorageList<T>();
        
        /// <summary>
        /// Изначальное число элементов. Назначение: подсчет прогресса.
        /// Значение задается на фабриках, например после чтения коллекции из файла.
        /// </summary>
        public int StartCount { get; set; }

        /// <summary>
        /// Создает новый список материалов.
        /// </summary>
        public MaterialsList()
        {
            MaterialsStorage = MaterialsStorageList;
        }

        /// <summary>
        /// Создает новый список материалов на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items">Элементы которые следует добавить в список</param>
        public MaterialsList(IEnumerable<T> items) : this() // Вызываем базовый конструктор сначала
        {
            foreach (var item in items)
                MaterialsStorageList.Add(item);
        }

        /// <summary>
        /// Возвращает следующий случайный материал из списка.
        /// </summary>
        public T GetNextRandom()
        {
            if (MaterialsStorage == null)
                return default(T);

            lock (MaterialsStorage)
                return MaterialsStorageList.GetNextRandom();
        }

        /// <summary>
        /// Устанавливает тип перечисления элементов списка.
        /// </summary>
        public MaterialsListIteration Iteration {
            get {
                lock (MaterialsStorage)
                    return MaterialsStorageList.Iteration;
            }
            set {
                lock (MaterialsStorage)
                    MaterialsStorageList.Iteration = value;
            }
        }

        /// <summary>
        /// Сбрасывает указатель текущего элемента списка на первый (нулевой индекс).
        /// </summary>
        public void ResetPointer()
        {
            lock (MaterialsStorageList)
                MaterialsStorageList.ResetPointer();
        }

        /// <summary>
        /// Проверяет существование элемента в списке.
        /// </summary>
        public bool Contains(T item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return MaterialsStorageList.Contains(item);
        }

        /// <summary>
        /// Удаляет элемент из списка.
        /// </summary>
        /// <param name="item">Элемент</param>
        /// <returns>Возвращает истину, если элемент был найден и удалён.</returns>
        public bool Remove(T item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return MaterialsStorageList.Remove(item);
        }
    }
}
