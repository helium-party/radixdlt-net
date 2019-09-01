using HeliumParty.RadixDLT.Configuration;
using HeliumParty.RadixDLT.Mapper;
using HeliumParty.RadixDLT.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT
{
    public class RadixApplicationAPIBuilder
    {
        private IRadixIdentity _identity;
        private RadixUniverseConfig _universe;//needs networklayer
        private IFeeMapper _feeMapper;
        //private IAtomToExecutedActionsMapper

        public RadixApplicationAPIBuilder Identity(IRadixIdentity identity)
        {
            _identity = identity;
            return this;
        }

        public RadixApplicationAPIBuilder Universe(RadixUniverseConfig universe)
        {
            _universe = universe;
            return this;
        }

        public RadixApplicationAPIBuilder FeeMapper(IFeeMapper feeMapper)
        {
            _feeMapper = feeMapper;
            return this;
        }

        public static RadixApplicationAPIBuilder Builder()
        {
            return new RadixApplicationAPIBuilder();
        }
    }

    
}
