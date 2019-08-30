using HeliumParty.RadixDLT.EllipticCurve.Managers;
using HeliumParty.RadixDLT.Identity.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Core.Identity
{
    public abstract class BaseRadixIdentity
    {
        protected IECKeyManager _keyManager;
        protected IEUIDManager _euidManager;
        public IECKeyManager KeyManager
        {
            get
            {
                if (_keyManager == null)
                    _keyManager = new ECKeyManager();
                return _keyManager;
            }
            set
            {
                _keyManager = value;
            }
        }

        public IEUIDManager EuidManager
        {
            get
            {
                if (_euidManager == null)
                    _euidManager = new EUIDManager();
                return _euidManager;
            }
            set
            {
                _euidManager = value;
            }
        }
    }
}
