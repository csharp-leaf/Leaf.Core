namespace Leaf.Core.Collections.Generic
{
    /// <summary>
    /// Абстрактная реализация потокобезопасной коллекции.
    /// </summary>
    /// <typeparam name="T">Тип хранимых объектов</typeparam>
    public abstract class LockedCollection<T>
    {
        /// <summary>
        /// Коллекция реализующая общие методы для работы с элементами.
        /// </summary>
        protected IStorage<T> Storage;

        /// <summary>
        /// Удаляет все элементы из коллекции.
        /// </summary>
        public virtual void Clear()
        {
            lock (Storage)
                Storage.Clear();
        }

        /// <summary>
        /// Доступное число элементов в коллекции.
        /// </summary>
        public virtual int Count
        {
            get {
                lock (Storage)
                    return Storage.Count;
            }
        }

        /// <summary>
        /// Возвращает следующий элемент коллекции. Если коллекция пуста будет возвращён null.
        /// </summary>
        public virtual T GetNext()
        {
            if (Storage == null)
                return default(T);

            lock (Storage)
                return Storage.GetNext();
        }

        /// <summary>
        /// Записывает следующий элемент коллекции в переменную.
        /// </summary>
        /// <param name="item">Переменная куда будет записан полученный элемент из коллекции</param>
        /// <returns>Вернет истину если элемент был возращен. Если коллеция уже пуста вернет ложь.</returns>
        public virtual bool GetNext(out T item)
        {
            item = GetNext();
            return item != null;
        }

        /// <summary>
        /// Добавляет элемент в коллекцию.
        /// </summary>
        /// <param name="item">Элемент</param>
        public virtual void AppendItem(T item)
        {
            if (Storage == null)
                return;

            lock (Storage)
                Storage.AppendItem(item);
        }        
    }
}
