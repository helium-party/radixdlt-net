using System.Threading.Tasks;
using HeliumParty.RadixDLT.Atoms;

namespace HeliumParty.RadixDLT.Identity
{
    public interface IRadixIdentity
    {
        Task<Atom> Sign(Atom atom);
        //Task<UnencryptedData> Decrypt;
    }
}