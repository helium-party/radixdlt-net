using System;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Particles;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Identity.Managers
{
    public class EUIDManager
    {
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
            throw new NotImplementedException();
        }
    }
}