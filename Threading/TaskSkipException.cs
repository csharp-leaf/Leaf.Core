using System;

namespace Leaf.Core.Threading
{
    /// <inheritdoc />
    /// <summary>
    /// Возникает если опциональная задача 
    /// Исключение не должно вызывать остановку конвеера
    /// </summary>
    [Serializable]
    public class TaskSkipException : Exception
    {
        public TaskSkipException(string message) : base(message)
        {
        }

        public TaskSkipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
