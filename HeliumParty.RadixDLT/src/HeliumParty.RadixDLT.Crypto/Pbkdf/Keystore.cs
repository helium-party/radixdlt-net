using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Pbkdf
{
    public class KeyStore
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "crypto")]
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
