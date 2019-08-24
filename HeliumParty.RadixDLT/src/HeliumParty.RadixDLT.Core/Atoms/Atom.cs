﻿using System.Collections.Generic;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Particles;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An atom is the fundamental atomic unit of storage on the ledger 
    /// (similar to a block in a blockchain) and defines the actions 
    /// that can be issued onto the ledger.
    /// </summary>
    public class Atom
    {
        public static string MetadataTimestampKey = "timestamp";
        public static string MetadataPowNonceKey = "powNonce";

        public List<ParticleGroup<Particle>> ParticleGroups { get; set; }
        public Dictionary<string, ECSignature> Signatures { get; set; }
        public Dictionary<string, string> MetaData { get; set; }
        public AID Id { get; set; }
    }
}