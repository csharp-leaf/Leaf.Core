using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections.FileBased
{
    /// <summary>
    /// Потокобезопасный список строк-материалов, читаемый из файла.
    /// </summary>
    public class FileMaterialsList : FileMaterialsBase
    {
        private readonly MaterialsStorageList<string> _materialsStorage = new MaterialsStorageList<string>();

        /// <inheritdoc />
        /// <summary>
        /// Создаёт потокобезопасный список строк-материалов из файла.
        /// </summary>
        public FileMaterialsList(string fileName, bool includeComments = false) : base(fileName, includeComments)
        {
            MaterialsStorage = _materialsStorage;
            BeforeReadFile += ResetPointer;
            ReadFromFile();
        }

        /// <summary>
        /// Возвращает следующий случайный материал из списка.
        /// </summary>
        public string GetNextRandom()
        {
            if (MaterialsStorage == null)
                return null;

            lock (MaterialsStorage)
                return _materialsStorage.GetNextRandom();
        }

        /// <summary>
        /// Устанавливает тип перечисления элементов списка.
        /// </summary>
        public MaterialsListIteration Iteration
        {
            get {
                lock (MaterialsStorage)
                    return _materialsStorage.Iteration;
            }
            set {
                lock (MaterialsStorage)
                    _materialsStorage.Iteration = value;
            }            
        }

        /// <summary>
        /// Сбрасывает указатель текущего элемента списка на первый (нулевой индекс).
        /// </summary>
        public void ResetPointer()
        {
            if (MaterialsStorage == null)
                return;
            
            lock (MaterialsStorage)
                _materialsStorage.ResetPointer();
        }

        /// <summary>
        /// Проверяет существование элемента в списке.
        /// </summary>
        /// <param name="item">Элемент</param>
        /// <returns>Возвращает истину, если элемент был найден в списке.</returns>
        public bool Contains(string item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return _materialsStorage.Contains(item);
        }

        /// <summary>
        /// Удаляет элемент из списка.
        /// </summary>
        /// <param name="item">Элемент</param>
        /// <returns>Возвращает истину, если элемент был найден и удалён.</returns>
        public bool Remove(string item)
        {
            if (MaterialsStorage == null)
                return false;

            lock (MaterialsStorage)
                return _materialsStorage.Remove(item);            
        }
    }
}
