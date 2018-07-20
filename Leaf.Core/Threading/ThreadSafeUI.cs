using System;
using System.Text;
using System.Threading;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Threading
{
    /// <summary>
    /// Делегат для вывода сообщения в лог, используется только для вызова в форме.
    /// </summary>
    /// <param name="message">Сообщение</param>
    public delegate void DFormLog(string message);

    /// <summary>
    /// Делегат для блокирования интерфейса, на время работы.
    /// </summary>
    /// <param name="enable">Включить элементы интерфейса</param>
    public delegate void DEnableUI(bool enable = true);
    /// <summary>
    /// Делегат для установления хода работы.
    /// </summary>
    /// <param name="current">Текущий рабочий элемент</param>
    /// <param name="total">Всего рабочих элементов</param>
    public delegate void DSetProgress(ulong current = 0, ulong total = 0);

    /// <summary>
    /// Представляет собой хранилище потокобезопасных методов для работы с интерфейсом.
    /// </summary>
    public abstract class ThreadSafeUI
    {
        /// <summary>
        /// Блокирование интерфейса, на время работы.
        /// </summary>
        public DEnableUI EnableUI;
        /// <summary>
        /// Установка хода работы.
        /// </summary>
        public DSetProgress SetProgress;

        /// <summary>
        /// Делегат который вызывает код формы, после того как сообщение лога было отформатировано, в соответствии с параметрами.
        /// </summary>
        public readonly DFormLog FormLog;

        /// <summary>
        /// Источник токена для прерывания работы.
        /// </summary>
        public CancellationTokenSource CancellationSource;

        /// <summary>
        /// Токен для проверки, была ли отменена работа пользователем. Используйте сокращение ThrowIfCanceled() и IsCanceled для работы с ним.
        /// </summary>
        public CancellationToken CancelToken => CancellationSource.Token;

        /// <summary>
        /// Обработчик событий при вызове Log.
        /// </summary>
        public event DFormLog OnLog;

        /// <summary>
        /// Возвращает истину если работа была отменена пользователем.
        /// </summary>
        public bool IsCanceled => CancelToken.IsCancellationRequested;

        /// <summary>
        /// Создает потокобезопасную реализацию для работы с интерфейсом.
        /// </summary>
        /// <param name="formLog">Делегат на запись в лог</param>
        protected ThreadSafeUI(DFormLog formLog) => FormLog = formLog;

        /// <summary>
        /// Бросает исключение, если работа была остановлена пользователем.
        /// </summary>
        /// <exception cref="OperationCanceledException">Бросает исключение если был вызвал manager.Stop()</exception>
        /// <exception cref="ObjectDisposedException">Бросает исключение если токен был удален</exception>
        public void ThrowIfCanceled()
        {
            CancelToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Уничтожает прошлый CancellationTokenSource и сбразывает CancelToken создавая новый.
        /// </summary>
        public void ResetCancelSource()
        {
            CancellationSource?.Dispose();
            CancellationSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Останавливает работу, вызывая завершение CancelToken и бросает исключение об отмене операции.
        /// </summary>
        /// <exception cref="OperationCanceledException">Возникает сразу после завершения вызова</exception>
        public void CancelAndThrow()
        {
            if (CancellationSource == null)
            {
                Log("ПРЕДУПРЕЖДЕНИЕ: Произошел Вызов отмены когда CancelTokenSource был уничтожен");
                return;
            }

            CancellationSource.Cancel();

            // throw new OperationCanceledException(CancelToken);
        }

        /// <summary>
        /// Ожидает указанное время или бросает исключение когда пользователь отменяет работу.
        /// </summary>
        /// <param name="millisecondsTimeout">Число миллисекунд для ожидания</param>
        /// <exception cref="OperationCanceledException">Бросает исключение если работа прервана пользователем</exception>
        public void SleepOrCancel(int millisecondsTimeout)
        {
            if (CancelToken.WaitHandle.WaitOne(millisecondsTimeout))
                CancelAndThrow();
        }

        /// <inheritdoc cref="SleepOrCancel(int)"/>
        /// <param name="timeout">Время ожидания</param>
        public void SleepOrCancel(TimeSpan timeout)
        {
            if (CancelToken.WaitHandle.WaitOne(timeout))
                CancelAndThrow();
        }

        /// <summary>
        /// Пишет сообщение в лог.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="showTime">Показывать дату сообщения</param>
        public void Log(string message, bool showTime = true)
        {
            var sb = new StringBuilder();
            if (showTime)
                sb.AppendFormat("{0:HH:mm:ss} | ", DateTime.Now);

            string threadName = Thread.CurrentThread.Name;
            if (threadName != null)
                sb.AppendFormat("{0} | ", threadName);

            sb.AppendLine(message);

            // Выводим в форму отформатированное сообщение
            string result = sb.ToString();
            FormLog(result);

            // Транслируем остальным лог
            OnLog?.Invoke(result);
        }

        /// <summary>
        /// Пишет форматируемое сообщение в лог.
        /// </summary>
        /// <param name="format">Форматируемая строка</param>
        /// <param name="args">Форматируемые параметры</param>
        public void Log(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Log(message);
        }
    }
}
