using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Particles;

namespace HeliumParty.RadixDLT.Identity.Managers
{
    public interface IEUIDManager
    {
        EUID GetEUID(byte[] hash);
        EUID GetEUID(ECPublicKey pubkey);
        EUID GetEUID(Particle particle);
        EUID GetEUID(RadixAddress address);
    }
}