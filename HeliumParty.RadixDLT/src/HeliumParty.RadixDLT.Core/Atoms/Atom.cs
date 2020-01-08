using System;
using System.Collections.Generic;
using System.Linq;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An atom is the fundamental atomic unit of storage on the ledger 
    /// (similar to a block in a blockchain) and defines the actions 
    /// that can be issued onto the ledger.
    /// </summary>
    [CborDiscriminator("radix.atom" , Policy = CborDiscriminatorPolicy.Always)]
    [JsonObject(ItemTypeNameHandling = TypeNameHandling.None)]
    public class Atom : SerializableObject
    {
        public static string MetadataTimestampKey = "timestamp";
        public static string MetadataPowNonceKey = "powNonce";
        
        public List<ParticleGroup> ParticleGroups { get; set; }

        [SerializationOutput(OutputMode.Api, OutputMode.Persist, OutputMode.Wire)]
        public SortedDictionary<string, ECSignature> Signatures { get; set; }

        public bool ShouldSerializeSignatures()
        {
            if (Signatures == null) return false;
            if (Signatures.Count == 0) return false;
            return true;
        }

        public SortedDictionary<string, string> MetaData { get; set; }

        [SerializationOutput(OutputMode.None)]
        public RadixHash Hash
        {
            get
            {
                //TODO should this manager be created via DI ?
                var manager = new DsonManager();
                return RadixHash.From(manager.ToDson(this, OutputMode.Hash));
            }
        }

        [SerializationOutput(OutputMode.None)]
        public AID Id => new AID(Hash, GetAllShards());

        public Atom() : this(0L) { }

        public Atom(long timestamp) : this(new List<ParticleGroup>(), timestamp) { }

        public Atom(ParticleGroup particleGroup, long timestamp) : 
            this(new List<ParticleGroup>{particleGroup}, timestamp) { }

        public Atom(List<ParticleGroup> particleGroups, long timestamp) : 
            this(particleGroups, new SortedDictionary<string, string> {{MetadataTimestampKey, timestamp.ToString()}}) { }

        public Atom(List<ParticleGroup> particleGroups, SortedDictionary<string, string> metaData)
        {
            ParticleGroups = particleGroups ?? throw new ArgumentNullException(nameof(particleGroups));
            MetaData = metaData ?? throw new ArgumentNullException(nameof(metaData));
        }

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

        public HashSet<long> GetAllShards()
        {
            return new HashSet<long>(GetAllParticles()
                .Select(sp => sp.Particle)
                .SelectMany(p => p.Destinations)
                .Select(d => d.Shard)
                .Distinct());
        }

        public HashSet<long> GetRequiredFirstShard()
        {
            var particles = GetAllParticles();
            if (particles.Any(sp => sp.Spin == Particles.Spin.Down))
                return new HashSet<long>(particles
                    .Where(sp => sp.Spin == Particles.Spin.Down)
                    .Select(sp => sp.Particle)
                    .SelectMany(p => p.Destinations)
                    .Select(d => d.Shard)
                    .Distinct());
            else
                return GetAllShards();
        }
    }
}