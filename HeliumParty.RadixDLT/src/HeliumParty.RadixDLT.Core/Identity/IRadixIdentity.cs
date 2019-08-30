using System.Threading.Tasks;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Data;
using HeliumParty.RadixDLT.EllipticCurve;

namespace HeliumParty.RadixDLT.Identity
{
    public interface IRadixIdentity
    {
        Task<Atom> Sign(Atom atom);

        // wrong java lib aproach. Decrypt is Identity dependent
        //Task<UnencryptedData> Decrypt();
        ECPublicKey PublicKey { get; }
    }
}