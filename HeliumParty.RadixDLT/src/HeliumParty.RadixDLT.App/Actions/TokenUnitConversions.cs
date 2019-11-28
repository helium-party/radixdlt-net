using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using HeliumParty.RadixDLT.Primitives;

namespace HeliumParty.RadixDLT.Actions
{
    public static class TokenUnitConversions
    {
        private static int POW_BASE = 10;
        private static int SUB_UNITS_POW10 = 18;
        private static UInt256 SUB_UNITS = BigInteger.Pow(POW_BASE, SUB_UNITS_POW10);
        private static BigDecimal SUB_UNITS_BIG_DECIMAL = BigDecimal.Pow(POW_BASE, SUB_UNITS_POW10);
        private static BigDecimal MINIMUM_GRANULARITY_BIG_DECIMAL = BigDecimal.Pow(1, -1 * SUB_UNITS_POW10);

        public static int GetTokenScale()
        {
            return SUB_UNITS_POW10;
        }

        public static BigDecimal GetSubUnits()
        {
            return SUB_UNITS_BIG_DECIMAL;
        }

        public static BigDecimal GetMinimumGranularity()
        {
            return MINIMUM_GRANULARITY_BIG_DECIMAL;
        }

        public static BigDecimal SubUnitsToUnits(UInt256 subunits)
        {
            return SubUnitsToUnits((BigInteger)subunits);
        }

        public static BigDecimal SubUnitsToUnits(BigInteger subunits)
        {
            return new BigDecimal(subunits, SUB_UNITS_POW10);
        }

        public static BigDecimal SubUnitsToUnits(long subunits)
        {
            return SubUnitsToUnits((BigInteger) subunits) ;
        }

        public static UInt256 UnitsToSubUnits(BigDecimal units)
        {
            return units * SUB_UNITS_BIG_DECIMAL;            
        }

        public static UInt256 UnitsToSubUnits(BigInteger units)
        {
            return UnitsToSubUnits(units);
        }

        public static UInt256 UnitsToSubUnits(long units)
        {
            return UnitsToSubUnits(units);
        }
    }
}
