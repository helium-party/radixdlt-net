using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Pbkdf
{
    public class Pbkdfparams
    {
        public int Iterations { get; set; }
        public int KeyLenght { get; set; }
        public string Digest { get; set; }
        public string Salt { get; set; }
    }
}
