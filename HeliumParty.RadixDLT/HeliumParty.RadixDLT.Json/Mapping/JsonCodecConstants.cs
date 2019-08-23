using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Mapping
{
    public static class JsonCodecConstants
    {
        // Length of string prefixes
        public const int PrefixLength = 5;

        // Dictionary which holds prefixes for JSON encoding and decoding
        public static readonly ImmutableDictionary<Type, string> JsonPrefixesDictionary =
            new Dictionary<Type, string>()
            { 
                {typeof(byte[]), ":byt:"},
                {typeof(EUID), ":uid:"},
                //{typeof(Hash), ":hsh:"}, TODO add once implemented
                {typeof(string), ":str:"},
                {typeof(RadixAddress), ":adr:"},
                {typeof(UInt256), ":u20:"},
                {typeof(RRI), ":rri:"},
                //{typeof(UInt384), ":u30:"},
                {typeof(AID), ":aid:"}
            }.ToImmutableDictionary();
    }
}