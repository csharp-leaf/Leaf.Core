using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Потокобезопасный список материалов.
    /// </summary>
    public class MaterialsList<T> : MaterialsBase<T>
    {
        protected readonly MaterialsStorageList<T> MaterialsStorageList = new MaterialsStorageList<T>();
        
        /// <summary>
        /// Создает новый список материалов или на основе перечислимой коллекции.
        /// </summary>
        /// <param name="items"></param>
        public MaterialsList(IEnumerable<T> items = null)
        {
            MaterialsStorage = MaterialsStorageList;

            if (items == null)
                return;

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
    }
}
