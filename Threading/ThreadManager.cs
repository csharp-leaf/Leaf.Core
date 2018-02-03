using System.Collections.Generic;
using System.Threading;

namespace Leaf.Core.Threading
{
    /// <summary>
    /// Обработчик события возникающего в случае завершения работы всех потоков.
    /// </summary>
    public delegate void ThreadsDoneEventHandler();
    /// <summary>
    /// Обработчик события возникающего перед запуском потоков.
    /// </summary>
    public delegate void ThreadsBeforeStartHandler();

    /// <summary>
    /// Реализация менеджера потоков, включая общие события, потокобезопасную работу с интерфейсом, остановку и принудительную остановку работы.
    /// </summary>
    public abstract class ThreadManager
    {
        /// <summary>
        /// Срабатывает в случае завершения работы всех потоков.
        /// </summary>
        protected event ThreadsDoneEventHandler Done;
        /// <summary>
        /// Срабатывает в перед запуском потоков.
        /// </summary>
        protected event ThreadsBeforeStartHandler BeforeStart;

        private readonly ThreadSafeUI _ui;
        private CancellationTokenSource _cancel;

        #region Thread Counters and Flags
        private readonly List<Thread> _threads = new List<Thread>();
        private int _activeThreads;
        // для исключения запуска нескольких событий Done
        private readonly object _lockerDone = new object(); 
        private bool _done;
        #endregion

        /// <summary>
        /// Создает менеджер для организации многопоточной работы.
        /// </summary>
        /// <param name="ui">Потокобезопасное хранилище для работы с интерфейсом</param>
        protected ThreadManager(ThreadSafeUI ui)
        {
            _ui = ui;
        }

        /// <summary>
        /// Запустить многопоточную работу.
        /// </summary>
        /// <param name="threadCount">Число потоков</param>
        /// <param name="args">Параметры, передаваемые в каждый поток при запуске</param>
        public void Start(uint threadCount, object args = null)
        {
            if (_activeThreads > 0)
            {
                _ui.Log("Сначала дождитесь завершения потоков");
                return;
            }
            if (threadCount <= 0)
            {
                _ui.Log("Укажите верное число потоков");
                return;
            }

            BeforeStart?.Invoke();

            // Сбрасываем флаг о завершении всех потоков
            _done = false;
            // Выключаем компоненты
            _ui.EnableUI(false);
            _ui.SetProgress(); // Сбрасываем полоску прогресса
            _threads.Clear();
            
            // Пересоздаем объект отмены            
            _cancel?.Dispose();
            _cancel = new CancellationTokenSource();
            _ui.CancelToken = _cancel.Token;

            for (uint i = 0; i < threadCount; i++)
            {
                var thread = new Thread(StartDoingWork)
                {
                    IsBackground = true,
                    Name = (i + 1).ToString()
                };
                thread.Start(args);
                _threads.Add(thread);
            }
        }

        /// <summary>
        /// Плавная остановка работы всех потоков, без потери результатов их работы.
        /// </summary>
        public void Stop()
        {
            _cancel.Cancel();
            _ui.Log("Идет плавная остановка всех потоков...");
        }

        /// <summary>
        /// Принудительная остановка всех потоков, с потерей результатов их работы.
        /// </summary>
        public void Abort()
        {
            _ui.Log("Принудительное завершение потоков...");

            // завершаем потоки
            foreach (Thread thread in _threads)
                thread.Abort();

            _threads.Clear();
            _activeThreads = 0;

            _ui.EnableUI();
            _ui.SetProgress();
        }

        /// <summary>
        /// Метод реализующий работу в рамках одного потока.
        /// </summary>
        /// <param name="args">Аргументы, переданнные при запуске потока</param>
        protected abstract void Do(object args);

        // Обертка для запуска Do()
        private void StartDoingWork(object args)
        {
            //
            // Для начала:
            // Увеличиваем счетчик активных потоков безопасно
            Interlocked.Increment(ref _activeThreads);

            // Делаем указанную работу, если отмены не было
            if (!_cancel.IsCancellationRequested)
                Do(args);

            //
            // В завершение:
            // Уменьшаем счетчик активных потоков безопасно
            int activeThreads = Interlocked.Decrement(ref _activeThreads);

            // Убираем поток из списка
            // + Избавляемся от бага когда 2 потока одновременно вызывают событие о завершении работы
            lock (_lockerDone)
            {
                if (_threads != null && _threads.Count > 0)
                    _threads.Remove(Thread.CurrentThread);

                if (activeThreads > 0 || _done)
                    return;

                _done = true;                    
                Done?.Invoke();

                _ui.SetProgress();
                _ui.EnableUI();
            }
        }
    }
}
