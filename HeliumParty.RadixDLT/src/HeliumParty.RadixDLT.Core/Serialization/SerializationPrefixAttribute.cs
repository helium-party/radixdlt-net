using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)] // TODO why allow properties and fields?
    public class SerializationPrefixAttribute : Attribute
    {
        public string Json { get; set; }
        public byte Dson { get; set; }

        public bool HasJsonPrefix => !string.IsNullOrWhiteSpace(Json);

        public bool HasDsonPrefix => Dson != 0x0;
    }
}
