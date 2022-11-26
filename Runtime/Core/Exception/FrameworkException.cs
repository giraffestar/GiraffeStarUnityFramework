using System;

namespace GiraffeStar
{
    public class FrameworkException : Exception
    {
        public override string Message { get; }

        public FrameworkException(string message)
        {
            Message = message;
        }
    }
}