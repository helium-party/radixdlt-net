using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HeliumParty.RadixDLT.Exceptions
{
    public class CryptographicException : ApplicationException
    {
        public CryptographicException()
        {
        }

        public CryptographicException(string message) : base(message)
        {
        }

        public CryptographicException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CryptographicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
