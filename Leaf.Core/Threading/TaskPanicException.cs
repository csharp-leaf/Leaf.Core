using System;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Threading
{
    /// <inheritdoc />
    /// <summary>
    /// Критическая ошибка конвеера. Вызывает его остановку.
    /// </summary>
    [Serializable]
    public class TaskPanicException : Exception
    {
        public TaskPanicException(string message) : base(message)
        {
        }

        public TaskPanicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
