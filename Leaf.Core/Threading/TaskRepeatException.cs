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
    public class TaskRepeatException : Exception
    {
        public TaskRepeatException(string message) : base(message)
        {
        }

        public TaskRepeatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
