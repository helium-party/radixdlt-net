﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An atom is the fundamental atomic unit of storage on the ledger 
    /// (similar to a block in a blockchain) and defines the actions 
    /// that can be issued onto the ledger.
    /// </summary>
    [CborDiscriminator("radix.atom" , Policy = CborDiscriminatorPolicy.Always)]
    public class Atom : SerializableObject
    {
        public static string MetadataTimestampKey = "timestamp";
        public static string MetadataPowNonceKey = "powNonce";
        
        public List<ParticleGroup> ParticleGroups { get; set; }

        [SerializationOutput(OutputMode.Api, OutputMode.Persist, OutputMode.Wire)]
        public Dictionary<string, ECSignature> Signatures { get; set; }

        public bool ShouldSerializeSignatures()
        {
            if (Signatures == null) return false;
            if (Signatures.Count == 0) return false;
            return true;
        }

        public Dictionary<string, string> MetaData { get; set; }

        [SerializationOutput(OutputMode.None)]
        public RadixHash Hash
        {
            get
            {
                var manager = new DsonManager();
                return RadixHash.From(manager.ToDson(this, OutputMode.Hash));
            }
        }

        public Atom() : this(0L) { }

        public Atom(long timestamp) : this(new List<ParticleGroup>(), timestamp) { }

        public Atom(ParticleGroup particleGroup, long timestamp) : 
            this(new List<ParticleGroup>{particleGroup}, timestamp) { }

        public Atom(List<ParticleGroup> particleGroups, long timestamp) : 
            this(particleGroups, new Dictionary<string, string> {{MetadataTimestampKey, timestamp.ToString()}}) { }

        public Atom(List<ParticleGroup> particleGroups, Dictionary<string, string> metaData)
        {
            ParticleGroups = particleGroups ?? throw new ArgumentNullException(nameof(particleGroups));
            MetaData = metaData ?? throw new ArgumentNullException(nameof(metaData));
        }

        //public AID Id { get; set; }

        // TODO: Add unit test.
        public IEnumerable<SpunParticle> GetAllParticles() => ParticleGroups.SelectMany(grp => grp.Particles);
        public IEnumerable<Particle> GetAllParticles(Spin spin) => ParticleGroups.SelectMany(grp => grp.GetParticlesWithSpin(spin));
        public IEnumerable<RadixAddress> GetAllAddresses()
        {
            return GetAllParticles()
                .Select(p => p.Particle)
                .SelectMany(p => p.GetShareables())
                .Distinct();
        }
    }
}