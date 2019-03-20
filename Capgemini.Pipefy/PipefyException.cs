using System;
using System.Runtime.Serialization;

namespace Capgemini.Pipefy
{
    [Serializable]
    internal class PipefyException : Exception
    {
        public PipefyException()
        {
        }

        public PipefyException(string message) : base(message)
        {
        }

        public PipefyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PipefyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}