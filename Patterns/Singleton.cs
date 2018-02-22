namespace Leaf.Core.Patterns
{
    /// <summary>
    /// Реализует паттерн "Одиночка".
    /// Экземляр класса будет создан по требованию и только в единственном экземпляре.
    /// </summary>
    /// <typeparam name="T">Тип которой следует использовать в единственном экземпляре</typeparam>
    public abstract class Singleton<T>
        where T : class, new()
    {
        private static volatile T _instance;

        // Внимание: SyncObject будет уникален для каждого типа T.
        // эта особенность позволяет эффективно реализовать Singleton абстрактно.
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object SyncObj = new object();

        /// <summary>
        /// Вернуть экземпляр класса.
        /// </summary>
        public static T Instance {
            get {
                if (_instance != null)
                    return _instance;

                lock (SyncObj)
                {
                    if (_instance == null)
                        _instance = new T();
                }

                return _instance;
            }
        }
    }
}
