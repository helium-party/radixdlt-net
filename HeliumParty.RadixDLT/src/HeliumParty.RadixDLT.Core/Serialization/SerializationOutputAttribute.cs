using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SerializationOutputAttribute : Attribute
    {
        public OutputMode[] ValidOn { get; }        

        public SerializationOutputAttribute(params OutputMode[] validOn)
        {
            ValidOn = validOn;
        }
    }
}
