using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Exceptions
{
    public class ProofOfWorkException : Exception
    {
        public ProofOfWorkException(string pow, string target)
            : base($"pow {pow} does not meet target {target}")
        {

        }
    }
}
