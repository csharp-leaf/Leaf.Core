namespace Leaf.Core.Collections.Generic
{
    public abstract class MaterialsBase<T>
    {        
        protected IMaterialsStorage<T> MaterialsStorage;

        /// <summary>
        /// Очистить все рабочие материалы.
        /// </summary>
        public virtual void Clear()
        {
            lock (MaterialsStorage)
                MaterialsStorage.Clear();
        }

        /// <summary>
        /// Доступное число материалов в данный момент.
        /// </summary>
        public virtual int Count
        {
            get {
                lock (MaterialsStorage)
                    return MaterialsStorage.Count;
            }
        }

        /// <summary>
        /// Возвращает следующий рабочий материал из коллекции. Если она пуста будет возвращён null.
        /// </summary>
        public virtual T GetNext()
        {
            if (MaterialsStorage == null)
                return default(T);

            lock (MaterialsStorage)
                return MaterialsStorage.GetNext();
        }

        /// <summary>
        /// Записывает следующий рабочий материал из коллекции в переменную. Возвращает истину в случае успеха.
        /// </summary>
        public virtual bool GetNext(out T item)
        {
            item = GetNext();
            return item != null;
        }

        /// <summary>
        /// Добавляет элемент в коллекцию.
        /// </summary>
        /// <param name="item">Материал</param>
        public virtual void AppendItem(T item)
        {
            if (MaterialsStorage == null)
                return;

            lock (MaterialsStorage)
                MaterialsStorage.AppendItem(item);
        }
    }
}
