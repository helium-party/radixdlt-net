using System;
using HeliumParty.DependencyInjection;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Primitives;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;

namespace HeliumParty.RadixDLT.Identity.Managers
{
    public class EUIDManager : IEUIDManager , ITransientDependency
    {

        private readonly IDsonManager _dsonManager;

        public EUIDManager(IDsonManager dsonManager)
        {
            _dsonManager = dsonManager;
        }

        public virtual EUID GetEUID(byte[] hash)
        {
            return new EUID(Arrays.SubArray(hash));
        }

        public virtual EUID GetEUID(ECPublicKey pubkey)
        {
            return GetEUID(RadixHash.From(pubkey.Base64Array).ToByteArray());
        }

        public virtual EUID GetEUID(RadixAddress address)
        {
            return GetEUID(address.ECPublicKey);
        }

        public virtual EUID GetEUID(Particle particle)
        {
            return GetEUID(RadixHash.From(_dsonManager.ToDson(particle,OutputMode.Hash)).ToByteArray());            
        }
    }
}