using System;

namespace Vision
{
    public class NotMatchedCameraQuantityException : Exception
    {
        public override string Message => base.Message;
        public NotMatchedCameraQuantityException() { }
        public NotMatchedCameraQuantityException(string message) : base(message) { }

    }
}