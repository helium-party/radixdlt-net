using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Exceptions
{
    public class MacMismatchException : CryptographicException
    {
        public MacMismatchException(byte [] expected, byte[] actual)
            :base("Mac mismatch, Expected " +
                 $"{Convert.ToBase64String(expected)} but was " +
                 $"{Convert.ToBase64String(actual)} ")
        {}
    }
}
