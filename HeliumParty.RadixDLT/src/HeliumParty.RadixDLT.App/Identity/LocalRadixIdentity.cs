using HeliumParty.RadixDLT.Atoms;
using HeliumParty.RadixDLT.Core.Identity;
using HeliumParty.RadixDLT.EllipticCurve;
using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Identity.Managers;
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

        public Task<Atom> Sign(Atom atom)
        {
            return new Task<Atom>(() => 
            {
                // to do : use our DSON lib to get a correct byte[] and hash it
                var signature = _keyManager.GetECSignature(_keyPair.PrivateKey, new byte[0]);
                var EUID = _euidManager.GetEUID(_keyPair.PublicKey);
                var signatures = new Dictionary<string, ECSignature>();
                signatures.Add(EUID.ToString(), signature);

                throw new NotImplementedException("DSON parsing is not yet implemented. unable to sign atom");

                return new Atom()
                {
                    ParticleGroups = atom.ParticleGroups,
                    MetaData = atom.MetaData,
                    Signatures = signatures
                };
            });
        }

        public LocalRadixIdentity(
            IECKeyManager keyManager, IEUIDManager euidManager, ECKeyPair keyPair)
            : base(keyManager, euidManager)
        {
            _keyPair = keyPair;            
        }
    }

    public class LocalExposedRadixIdentity : LocalRadixIdentity
    {
        public ECKeyPair KeyPair => _keyPair;
        public LocalExposedRadixIdentity(IECKeyManager keyManager, IEUIDManager euidManager, ECKeyPair keyPair) 
            : base(keyManager, euidManager, keyPair)
        {
        }
    }
}
