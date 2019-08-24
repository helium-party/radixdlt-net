using System.Collections.Generic;
using HeliumParty.RadixDLT.Identity;

namespace HeliumParty.RadixDLT.Particles
{
    public interface IAccountable
    {
        HashSet<RadixAddress> Addresses { get; }
    }
}