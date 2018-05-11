using System;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Extensions.String
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение возникающее в Ex методах при поиске строки или нескольких между двух подстрок.
    /// </summary>
    public class StringBetweenException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Исключение говорящее о том что не удалось найти одну или несколько подстрок между двумя подстроками.
        /// </summary>
        public StringBetweenException() { }

        /// <inheritdoc />
        /// <inheritdoc cref="StringBetweenException()"/>        
        public StringBetweenException(string message) : base(message) {}

        /// <inheritdoc />
        /// <inheritdoc cref="StringBetweenException()"/>
        public StringBetweenException(string message, Exception innerException) : base(message, innerException) {}
    }
}