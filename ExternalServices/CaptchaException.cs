using System;

namespace Leaf.Core.ExternalServices
{
    class CaptchaException : Exception
    {
        public CaptchaException(string message) : base(message)
        {
            
        }
    }
}
