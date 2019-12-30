using Dahomey.Cbor.Attributes;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;
using HeliumParty.RadixDLT.Serialization.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HeliumParty.RadixDLT.Universe
{
    [CborDiscriminator("radix.universe", Policy = CborDiscriminatorPolicy.Always)]
    public class RadixUniverseConfig : SerializableObject
    {
        #region Public Members
        [SerializationOutput(OutputMode.Api, OutputMode.Persist, OutputMode.Wire)]
        public long Magic { get; }

        [SerializationOutput(OutputMode.All)]
        public int Port { get; }

        [SerializationOutput(OutputMode.All)]
        public string Name { get; }

        [SerializationOutput(OutputMode.All)]
        public string Description { get; }

        [SerializationOutput(OutputMode.All)]
        public long Timestamp { get; }

        [SerializationOutput(OutputMode.All)]
        public ImmutableList<Atom> Genesis { get; }

        [SerializationOutput(OutputMode.All)]
        public RadixUniverseType Type { get; }

        [CborProperty("creator")]
        [JsonProperty("creator")]
        [SerializationOutput(OutputMode.All)]
        public ECPublicKey SystemPublicKey { get; }
        #endregion

        public RadixUniverseConfig(
            string name,
            string description,
            ECPublicKey systemPublicKey,
            long timestamp,
            RadixUniverseType type,
            int port,
            int magic,
            List<Atom> genesis)
        {
            Name = name;
            Description = description;
            SystemPublicKey = systemPublicKey;
            Timestamp = timestamp;
            Type = type;
            Port = port;
            Magic = magic;
            Genesis = ImmutableList.Create(genesis.ToArray());
        }

        public static RadixUniverseConfig FromBytes(byte[] bytes)
        {
            var manager = new JsonManager();
            var json = RadixConstants.StandardEncoding.GetString(bytes);
            return manager.FromJson<RadixUniverseConfig>(json);
        }
               
        public RadixHash GetHash()
        {
            var manager = new DsonManager();    // TODO: Should probably a DependencyInjection provided manager
            return RadixHash.From(manager.ToDson(this, OutputMode.Hash));
        }

        public EUID GetHId() => new EUID(GetHash().ToByteArray());

        public byte GetMagicByte() => (byte)(Magic & 0xff);

        public RadixAddress GetSystemAddress() => new RadixAddress((int)Magic, SystemPublicKey);

        public override string ToString() => $"{Name} {Magic} ";
        public override int GetHashCode() => GetHash().GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is RadixUniverseConfig config)
                return this.GetHash().Equals(config.GetHash());

            return false;
        }
    }
}
