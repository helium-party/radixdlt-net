using HeliumParty.RadixDLT.EllipticCurve.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.App.Identity
{
    public class BaseRadixIdentity
    {
        public IECKeyManager KeyManager { get; set; }
    }
}
