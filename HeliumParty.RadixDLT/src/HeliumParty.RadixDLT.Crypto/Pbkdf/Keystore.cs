using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Pbkdf
{
    public class Keystore
    {
        public string Id { get; set; }
        public CryptoDetails CryptoDetails { get; set; }

    }

    public class CryptoDetails
    {
        public string Cipher { get; set; }
        public CipherParams CipherParams { get; set; }
        public string CipherText { get; set; }
        public Pbkdfparams Pbkdfparams { get; set; }
        public string Mac { get; set; }

    }
}
