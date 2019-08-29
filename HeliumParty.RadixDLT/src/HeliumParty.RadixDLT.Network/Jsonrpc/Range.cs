using System;
using System.Numerics;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// A class for working with the delta of two values
    /// </summary>
    /// <typeparam name="T">Any value type of which a difference to another value can be built</typeparam>
    public class Range<T> where T : IComparable<T>   // TODO Missing Extension..
    {
        /// <summary>
        /// The values for calculating the delta
        /// </summary>
        private T _Low, _High;

        /// <summary>
        /// Constructor with values to set
        /// </summary>
        /// <param name="low">The lower value of the range</param>
        /// <param name="high">The higher value of the range</param>
        /// <exception cref="ArgumentException">'Low' must be less than 'high'!</exception>
        public Range(T low, T high)
        {
            if (low.CompareTo(high) > 0)
                throw new ArgumentException($"{nameof(low)} must be lesser than {nameof(high)}");

            _Low = low;
            _High = high;
        }
        
        /// <summary>
        /// Sets the lower value of the range
        /// </summary>
        /// <param name="low">The value to set as 'low'</param>
        /// <exception cref="ArgumentException">'Low' must be less than 'high'!</exception>
        protected void SetLow(T low)
        {
            if (low.CompareTo(_High) > 0)
                throw new ArgumentException($"{nameof(low)} must be lesser than the already stored high value");

            _Low = low;
        }

        /// <summary>
        /// Sets the higher value of the range
        /// </summary>
        /// <param name="high">The value to set as 'high'</param>
        /// <exception cref="ArgumentException">'High' must be greater than 'low'!</exception>
        protected void SetHigh(T high)
        {
            if (_Low.CompareTo(high) > 0)
                throw new ArgumentException($"{nameof(high)} must be greater than the already stored low value");

            _High = high;
        }

        /// <summary>
        /// Returns the stored lower value
        /// </summary>
        /// <returns>The stored lower value</returns>
        public T GetLow() => _Low;

        /// <summary>
        /// Returns the stored higher value
        /// </summary>
        /// <returns>The stored higher value</returns>
        public T GetHigh() => _High;

        /// <summary>
        /// Gets the delta between the high and low value
        /// </summary>
        /// <returns>delta of high and low</returns>
        public T GetSpan()
        {
            object objHigh = _High;
            object objLow = _Low;

            // As we cannot implicitely convert from T to the value types and reversely,                        // TODO: Check if this actually works!
            // we'll use the 'object' class as middle layer
            if (typeof(T) == typeof(float))
                return (T)(object)((float)objHigh - (float)objLow);

            else if (typeof(T) == typeof(double))
                return (T)(object)((double)objHigh - (double)objLow);

            else if (typeof(T) == typeof(BigDecimal))
                return (T)(object)((Utils.BigDecimal)objHigh - (Utils.BigDecimal)objLow);

            else if (typeof(T) == typeof(BigInteger))
                return (T)(object)((BigInteger)objHigh - (BigInteger)objLow);

            else if (typeof(T) == typeof(long))
                return (T)(object)((long)objHigh - (long)objLow);

            else if (typeof(T) == typeof(int))
                return (T)(object)((int)objHigh - (int)objLow);

            else if (typeof(T) == typeof(short))
                return (T)(object)((short)objHigh - (short)objLow);

            else if (typeof(T) == typeof(byte))
                return (T)(object)((byte)objHigh - (byte)objLow);

            throw new NotSupportedException("Cannot calculate the range with type " + typeof(T).ToString());    // TODO: Check exception text thrown - will correct type be printed?
        }

        /// <summary>
        /// Contains whether specified value is contained in the range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>Whether range contains value</returns>
        public bool Contains(T value)
        {
            return Contains(value, value);
        }

        /// <summary>
        /// Returns whether both specified values are contained this instances range
        /// </summary>
        /// <param name="low">The low value which should be greater/equal stored low value</param>
        /// <param name="high">The high value which should be lesser/equal stored high value</param>
        /// <returns>Whether range contains values</returns>
        public bool Contains(T low, T high)
        {
            if (_Low.CompareTo(low) <= 0 && _High.CompareTo(high) >= 0)
                return true;

            return false;
        }
    }
}
