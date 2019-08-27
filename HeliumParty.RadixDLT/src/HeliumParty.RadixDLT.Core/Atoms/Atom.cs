using System;
using System.Collections.Generic;
using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Identity;
using Newtonsoft.Json;

namespace HeliumParty.RadixDLT.Atoms
{
    /// <summary>
    /// An atom is the fundamental atomic unit of storage on the ledger 
    /// (similar to a block in a blockchain) and defines the actions 
    /// that can be issued onto the ledger.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Atom
    {
        public static string MetadataTimestampKey = "timestamp";
        public static string MetadataPowNonceKey = "powNonce";

        [CborProperty("particleGroups"), JsonProperty(PropertyName = "particleGroups")]
        public List<ParticleGroup> ParticleGroups { get; set; } = new List<ParticleGroup>();

        [CborProperty("signatures"), JsonProperty(PropertyName = "signatures")]
        public Dictionary<string, ECSignature> Signatures { get; set; } = new Dictionary<string, ECSignature>();

        [CborProperty("metaData"), JsonProperty(PropertyName = "metaData")]
        public Dictionary<string, string> MetaData { get; set; }

        public AID Id { get; set; }

        public Atom() : this(0) { }
        
        public Atom(long timestamp) : this(new List<ParticleGroup>(), timestamp) { }

        public Atom(ParticleGroup particleGroup, long timestamp) : this(new List<ParticleGroup>() { particleGroup }, timestamp) { }

        public Atom(List<ParticleGroup> particleGroups, long timestamp) : this(particleGroups,
            new Dictionary<string, string>() { { MetadataTimestampKey, timestamp.ToString() } }) { }
        
        public Atom(List<ParticleGroup> particleGroups, Dictionary<string, string> metadata)
        {
            if (particleGroups == null)
                throw new ArgumentNullException(nameof(particleGroups) + " is required");
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata) + " is required");

            ParticleGroups.AddRange(particleGroups);
            MetaData = metadata;
        }

        public Atom(List<ParticleGroup> particleGroups, Dictionary<string, string> metadata, EUID signatureId, ECSignature signature) : this(particleGroups, metadata)
        {
            if (signatureId == null)
                throw new ArgumentNullException(nameof(signatureId) + " is required");
            if (signature == null)
                throw new ArgumentNullException(nameof(signature) + " is required");

            Signatures.Add(signatureId.ToString(), signature);
        }
    }
}