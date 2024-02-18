using System;

namespace AutoSharp.Exceptions
{

    [Serializable]
    public class ChatException : Exception
    {
        public ChatException() { }
        public ChatException(string message) : base(message) { }
        public ChatException(string message, Exception inner) : base(message, inner) { }
        protected ChatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
