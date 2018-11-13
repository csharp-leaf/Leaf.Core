using System;
using System.Collections.Generic;
using System.Threading;

// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Threading
{
    /// <inheritdoc />
    /// <summary>
    /// Обработчик события возникающего в случае завершения работы всех потоков.
    /// </summary>
    /// <summary>
    /// Обработчик события возникающего перед запуском потоков.
    /// </summary>
    /// <summary>
    /// Реализация менеджера потоков, включая общие события, потокобезопасную работу с интерфейсом, остановку и принудительную остановку работы.
    /// </summary>
    public abstract class ThreadManager : IDisposable
    {
        /// <summary>
        /// Возвращает число активных потоков.
        /// </summary>
        public int ActiveThreads => _activeThreads;

        /// <summary>
        /// Возвращает истину если работа в процессе.
        /// </summary>
        public bool IsWorking { get; private set; }

        /// <summary>
        /// Срабатывает в случае завершения работы всех потоков.
        /// </summary>
        protected Action Done;

        /// <summary>
        /// Срабатывает в перед запуском потоков.
        /// </summary>
        protected Action BeforeStart;

        /// <summary>
        /// Отвечает за то, надо ли именовать потоки номерами в порядке создания.
        /// Значение false позволит задать имена потокам позже, если это требуется.
        /// </summary>
        protected bool AppendThreadNames { get; set; } = true;


        #region Private Variables

        private readonly ThreadSafeUI _ui;

        private readonly List<Thread> _threads = new List<Thread>();
        private int _activeThreads; // число активных потоков

        // для исключения запуска нескольких событий Done
        private readonly object _lockerDone = new object();

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
        /// <param name="args">Массив уникальных параметров для каждого потока</param>
        public void Start(uint threadCount, object[] args = null)
        {
            // Валидация
            if (IsWorking || threadCount == 0)
                return;

            // Пересоздаем объект отмены
            _ui.ResetCancelSource();

            BeforeStart?.Invoke();

            // Сбрасываем флаг о завершении всех потоков
            IsWorking = true;

            // Выключаем компоненты
            _ui.EnableUI?.Invoke(false);
            _ui.SetProgress?.Invoke(); // Сбрасываем полоску прогресса

            _threads.Clear();
            for (uint i = 0; i < threadCount; i++)
            {
                var thread = new Thread(StartDoingWork) {
                    IsBackground = true
                };

                if (AppendThreadNames)
                    thread.Name = (i + 1).ToString();

                if (args == null || i >= args.Length)
                    thread.Start();
                else
                    thread.Start(args[i]);

                _threads.Add(thread);
            }
        }

        /// <summary>
        /// Плавная остановка работы всех потоков, без потери результатов их работы.
        /// </summary>
        public void Stop()
        {
            if (!IsWorking)
                return;

            _ui.CancelAndThrow();
            _ui.Log("Идет плавная остановка всех потоков...");
        }

        /// <summary>
        /// Принудительная остановка всех потоков, с потерей результатов их работы.
        /// </summary>
        public void Abort()
        {
            if (!IsWorking)
                return;

            _ui.Log("Принудительное завершение потоков...");

            // завершаем потоки
            foreach (var thread in _threads)
                thread.Abort();

            _threads.Clear();
            _activeThreads = 0;

            _ui.EnableUI();
            _ui.SetProgress();
        }

        /// <summary>
        /// Метод реализующий работу в рамках одного потока.
        /// </summary>
        /// <param name="args">Аргументы, переданные при запуске потока</param>
        protected abstract void Do(object args);

        // Обертка для запуска Do()
        private void StartDoingWork(object args)
        {
            //
            // Для начала:
            // Увеличиваем счетчик активных потоков безопасно
            Interlocked.Increment(ref _activeThreads);

            // Делаем указанную работу, если отмены не было
            if (!_ui.IsCanceled)
                Do(args);

            // Уменьшаем счетчик активных потоков безопасно
            int activeThreads = Interlocked.Decrement(ref _activeThreads);

            // Убираем поток из списка
            // + Избавляемся от бага когда 2 потока одновременно вызывают событие о завершении работы
            lock (_lockerDone)
            {
                if (_threads != null && _threads.Count > 0)
                    _threads.Remove(Thread.CurrentThread);

                if (activeThreads > 0)
                    return;

                IsWorking = false;
            }

            Done?.Invoke();

            _ui.SetProgress?.Invoke();
            _ui.EnableUI?.Invoke();
        }

        #region IDisposable Support

        private bool _disposed; // Для определения избыточных вызовов

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // ReSharper disable once UseNullPropagation
            if (disposing)
                _ui.CancellationSource?.Dispose();

            _disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
        }

        #endregion
    }
}