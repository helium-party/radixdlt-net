using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles
{
    public interface IOwnable
    {
        RadixAddress Address { get; }
    }
}