using System;
using System.Text;

namespace Leaf.Core.Extensions.System
{
    /// <summary>
    /// Делегат, обрабатывающий исключение внутри нескольких исключений (объединённых).
    /// </summary>
    /// <param name="ex">Исключение</param>
    /// <returns>вернет истину если ошибка была обработана и не следует бросать исключение выше.</returns>
    public delegate bool DProcessAggregated(Exception ex);

    // ReSharper disable once UnusedMember.Global
    public static class ExceptionExtensions
    {
        public static int MaxExceptionInnerLevelDetails
        {
            get => _maxExceptionInnerLevelDetails;
            set {
                if (value <= 0)
                    throw new ArgumentException("Invalid value. It must be greater than 0", nameof(MaxExceptionInnerLevelDetails));

                _maxExceptionInnerLevelDetails = value;
            }
        }
        private static int _maxExceptionInnerLevelDetails = 3;

        /// <summary>
        /// Возвращает детальное сообщение исключения с включением внутренней ошибки, если она присутствует.
        /// </summary>
        /// <param name="ex">Исключение</param>
        /// <param name="stackTrace">Включить ли стек вызовов в сообщение</param>
        /// <returns>Полное сообщение об исключении</returns>
        public static string GetDetailedMessage(this Exception ex, bool stackTrace = true)
        {
            //if (maxInnerLevel <= 0)
                //maxInnerLevel = MaxExceptionInnerLevelDetails;
                //throw new ArgumentException("Invalid value. It must be greater than 0", nameof(maxInnerLevel));
            
            var sb = new StringBuilder();
            int innerLevel = 1;

            sb.AppendFormat("Exception: \"{0}\"", ex.GetType().Name);
            if (ex.Message != null) {
                sb.AppendFormat(". Message: {0}", ex.Message);
            }
            if (stackTrace)
            {
                sb.AppendLine();
                sb.Append(ex.StackTrace);
            }

            var innerException = ex.InnerException;

            while (innerException != null && innerLevel <= MaxExceptionInnerLevelDetails)
            {
                string innerType = innerException.GetType().ToString();
                sb.AppendLine();
                sb.AppendFormat("EX #{0} InnerException: \"{1}\"", innerLevel, innerType);
                if (innerException.Message != null) {
                    sb.AppendFormat(". Message: {0}", innerException.Message);
                }

                innerException = innerException.InnerException;
                ++innerLevel;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Обрабатывает несколько исключений (агрегация) через делегат-обработчик.
        /// </summary>
        /// <param name="ex">Возможно агрегированное исключение</param>
        /// <param name="handler">Обработчик</param>
        /// <returns>вернет истину если ошибка была обработана и не следует бросать исключение выше.</returns>
        public static bool ProcessAggregated(this Exception ex, DProcessAggregated handler)
        {
            if (!(ex is AggregateException ag)) 
                return handler(ex);

            // Process all inner exceptions - if any false - return false
            foreach (var innerException in ag.Flatten().InnerExceptions)
            {
                if (!handler(innerException))
                    return false;
            }

            return true;
        }
    }
}