using System;
using System.Linq;

namespace HeliumParty.RadixDLT.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SerializationOutputAttribute : Attribute
    {
        public OutputMode[] ValidOn { get; }        

        public SerializationOutputAttribute(params OutputMode[] validOn)
        {
            if (validOn.Contains(OutputMode.None) && validOn.Length != 1)
                throw new InvalidOperationException("If OutputMode.None is used, there can not be another OutputMode");
            ValidOn = validOn;
        }
    }
}
