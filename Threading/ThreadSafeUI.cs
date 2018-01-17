using System;
using System.Text;
using System.Threading;

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
    public delegate void DEnableUi(bool enable = true);
    /// <summary>
    /// Делегат для установления хода работы.
    /// </summary>
    /// <param name="current">Текущий рабочий элемент</param>
    /// <param name="total">Всего рабочих элементов</param>
    public delegate void DSetProgress(int current = 0, int total = 0);

    /// <summary>
    /// Представляет собой хранилище потокобезопасных методов для работы с интерфейсом.
    /// </summary>
    public abstract class ThreadSafeUi
    {
        /// <summary>
        /// Блокирование интерфейса, на время работы.
        /// </summary>
        public DEnableUi EnableUi;
        /// <summary>
        /// Установка хода работы.
        /// </summary>
        public DSetProgress SetProgress;

        /// <summary>
        /// Делегат который вызывает код формы, после того как сообщение лога было отформатировано, в соответствии с параметрами.
        /// </summary>
        public readonly DFormLog FormLog;
        /// <summary>
        /// Токен для проверки, была ли отменена работа пользователем. Используйте сокращение ThrowIfCanceled() и IsCanceled для работы с ним.
        /// </summary>
        public CancellationToken CancelToken;

        /// <summary>
        /// Создает потокобезопасную реализацию для работы с интерфейсом.
        /// </summary>
        /// <param name="log">Метод вызываемый формой, с обработкой Invoke, для вывода сообщения в лог</param>
        /// <param name="formLog">Делегат на запись в лог</param>
        protected ThreadSafeUi(DFormLog formLog) => FormLog = formLog;

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
        /// Возвращает истину если работа была отменена пользователем.
        /// </summary>
        public bool IsCanceled => CancelToken.IsCancellationRequested;

        /// <summary>
        /// Пишет сообщение в лог
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="showTime">Показывать дату сообщения</param>
        public void Log(string message, bool showTime = true)
        {
            string threadName = Thread.CurrentThread.Name;

            var sb = new StringBuilder();
            if (showTime)
                sb.AppendFormat("[{0:HH:mm:ss}] ", DateTime.Now);

            if (threadName != null)
                sb.AppendFormat("[№{0}] ", threadName);

            sb.AppendLine(message);

            // Выводим в форму отформатированное сообщение
            FormLog(sb.ToString());
            sb.Clear();
        }
    }
}
