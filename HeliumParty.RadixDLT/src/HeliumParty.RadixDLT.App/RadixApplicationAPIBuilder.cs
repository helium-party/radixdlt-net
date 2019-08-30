using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public class RadixApplicationAPIBuilder
    {
        private IRadixIdentity _identity;
        private object _universe;//needs networklayer

        public RadixApplicationAPIBuilder Identity(IRadixIdentity identity)
        {
            _identity = identity;
            return this;
        }

        public RadixApplicationAPIBuilder Universe(object universe)
        {
            _universe = universe;
            return this;
        }

        public static RadixApplicationAPIBuilder Builder()
        {
            return new RadixApplicationAPIBuilder();
        }
    }

    
}
