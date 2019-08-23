using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Mapping
{
    public static class DsonCodecConstants
    {
        // Dictionary which holds prefixes for DSON encoding and decoding
        public static readonly ImmutableDictionary<Type, byte> DsonPrefixesDictionary =
            new Dictionary<Type, byte>
            {
                {typeof(byte[]), 0x01},
                {typeof(EUID), 0x02},
                //{typeof(Hash), 0x03}, TODO add once implemented
                {typeof(RadixAddress), 0x04},
                {typeof(UInt256), 0x05},
                {typeof(RRI), 0x06},
                //{typeof(UInt384), 0x07},
                {typeof(AID), 0x08}
            }.ToImmutableDictionary();
    }
}