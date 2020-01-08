using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Core.Identity;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Hashing;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;
using HeliumParty.RadixDLT.Serialization;
using HeliumParty.RadixDLT.Serialization.Dson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeliumParty.RadixDLT.Identity
{
    public class LocalRadixIdentity : BaseRadixIdentity, IRadixIdentity
    {
        protected readonly ECKeyPair _keyPair;
        public ECPublicKey PublicKey => _keyPair.PublicKey;

        private readonly IDsonManager _dsonManager;

        public LocalRadixIdentity(
            IECKeyManager keyManager,
            IEUIDManager euidManager,
            IDsonManager dsonManager,
            ECKeyPair keyPair)
            : base(keyManager, euidManager)
        {
            _keyPair = keyPair;
            _dsonManager = dsonManager;
        }

        public async Task<Atom> Sign(Atom atom)
        {
            var hash = RadixHash.From(await _dsonManager.ToDsonAsync(atom, OutputMode.Hash));

            var signature = _keyManager.GetECSignature(_keyPair.PrivateKey, hash.ToByteArray());
            var EUID = _euidManager.GetEUID(_keyPair.PublicKey);
            var signatures = new SortedDictionary<string, ECSignature>();
            signatures.Add(EUID.ToString(), signature);

            return new Atom()
            {
                ParticleGroups = atom.ParticleGroups,
                MetaData = atom.MetaData,
                Signatures = signatures
            };
        }
    }

    public class LocalExposedRadixIdentity : LocalRadixIdentity
    {
        public ECKeyPair KeyPair => _keyPair;
        public LocalExposedRadixIdentity(IECKeyManager keyManager, IEUIDManager euidManager, IDsonManager dsonManager, ECKeyPair keyPair) 
            : base(keyManager, euidManager, dsonManager, keyPair)
        {
        }
    }
}
