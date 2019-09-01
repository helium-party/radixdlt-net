using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Mapper
{
    public interface IFeeMapper
    {
        KeyValuePair<IDictionary<string,string>,IList<ParticleGroup>> Map(Atom atom, int magic, ECPublicKey key);
    }
}
