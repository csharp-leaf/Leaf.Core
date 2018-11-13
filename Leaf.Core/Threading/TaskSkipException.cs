using System;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Threading
{
    /// <inheritdoc />
    /// <summary>
    /// Возникает если опциональная задача 
    /// Исключение не должно вызывать остановку конвейера
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
