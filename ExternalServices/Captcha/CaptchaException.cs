using System;

namespace Leaf.Core.ExternalServices.Captcha
{
    [Serializable]
    public class CaptchaException : Exception
    {
        /// <summary>
        /// Возникает когда сервис не смог решить каптчу.
        /// </summary>
        public CaptchaException() { }

        /// <inheritdoc cref="CaptchaException"/>
        /// <param name="message">Сообщение об ошибке при решении каптчи</param>
        public CaptchaException(string message) : base(message)
        { 
        }
    }
}
