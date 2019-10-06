using System;
using System.Collections.Generic;
using System.Text;
using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.Hashing;

namespace HeliumParty.RadixDLT.Mappers
{
    public class PowFeeMapper : IFeeMapper
    {
        private readonly Func<Atom, RadixHash> _hasher;
        private readonly ProofOfWorkBuilder _powBuilder;
        private const int LEADING = 16;
        public KeyValuePair<IDictionary<string, string>, IList<ParticleGroup>> Map(Atom atom, int magic, ECPublicKey key)
        {
            throw new NotImplementedException();
        }


    }
}
