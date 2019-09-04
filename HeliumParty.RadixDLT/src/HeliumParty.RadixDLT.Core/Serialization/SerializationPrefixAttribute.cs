using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class SerializationPrefixAttribute : Attribute
    {
        public string Json { get; set; }
        public byte Dson { get; set; }
        public bool HasJsonPrefix {
            get
            {
                return !string.IsNullOrWhiteSpace(Json);
            }
        }
        public bool HasDsonPrefix
        {
            get
            {
                return !(Dson == 0x0);
            }
        }

    }
}
