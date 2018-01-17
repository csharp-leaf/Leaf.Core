using System;

namespace Leaf.Core.Threading
{
    internal class TaskPanicException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Возникает при критической ошибке конвеера и вызывает его остановку.
        /// </summary>
        public TaskPanicException(string message) : base(message)
        {
        }

        public TaskPanicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
