using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HeliumParty.RadixDLT.Address
{
    public class RadixUniverseConfig // TODO: SerializableObject?? --> We might not want to have it in this project!
    {
        public int Magic { get; }
        public ImmutableList<Atom> Genesis { get; }
        public ECPublicKey Creator { get; }
        public string Name { get; }
        public string Description { get; }

        private long _Timestamp;
        private RadixUniverseType _Type;
        private long _Port;

        // TODO: Why no arg constructor for serialization?
        public RadixUniverseConfig() { }

        public RadixUniverseConfig(
            string name,
            string description,
            ECPublicKey creator,
            long timestamp,
            RadixUniverseType type,
            long port,
            int magic, 
            List<Atom> genesis) 
        {
            Name = name;
            Description = description;
            Creator = creator;
            _Timestamp = timestamp;
            _Type = type;
            _Port = port;
            Magic = magic;
            Genesis = ImmutableList.Create(genesis.ToArray());
        }

        /// <summary>
        /// Returns the magic as a single byte
        /// </summary>
        /// <returns>The magic as single byte</returns>
        public byte GetMagicByte() => (byte)(Magic & 0xFF);

        /// <summary>
        /// Creates a new <see cref="RadixAddress"/> instance for the current <see cref="RadixUniverseConfig"/>
        /// </summary>
        public RadixAddress GetAddress() => new RadixAddress(Magic, Creator);

        public RadixHash GetHash() => throw new NotImplementedException();        // TODO: Serialize to DSON? Need more info on this...

        public EUID GetHID() => new EUID(GetHash().ToByteArray());

        public override string ToString() => $"{Name} {Magic} {GetHID()}";
        public override int GetHashCode() => GetHash().GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj != null && obj is RadixUniverseConfig config)
                return GetHash().Equals(config.GetHash());

            return false;
        }
    }
}
