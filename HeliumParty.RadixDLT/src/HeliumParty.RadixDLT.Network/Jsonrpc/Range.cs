using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    /// <summary>
    /// A class for working with the delta of two values
    /// </summary>
    /// <typeparam name="T">Any value type of which a difference to another value can be built</typeparam>
    public class Range<T> where T : IComparable<T>   // TODO Missing Extension..
    {
        public T Low { get; private set; }
        public T High { get; private set; }

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

            Low = low;
            High = high;
        }
        
        /// <summary>
        /// Sets the lower value of the range
        /// </summary>
        /// <param name="low">The value to set as 'low'</param>
        /// <exception cref="ArgumentException">'Low' must be less than 'high'!</exception>
        protected void SetLow(T low)
        {
            if (low.CompareTo(High) > 0)
                throw new ArgumentException($"{nameof(low)} must be lesser than the already stored high value");

            Low = low;
        }

        /// <summary>
        /// Sets the higher value of the range
        /// </summary>
        /// <param name="high">The value to set as 'high'</param>
        /// <exception cref="ArgumentException">'High' must be greater than 'low'!</exception>
        protected void SetHigh(T high)
        {
            if (Low.CompareTo(high) > 0)
                throw new ArgumentException($"{nameof(high)} must be greater than the already stored low value");

            High = high;
        }

        /// <summary>
        /// Gets the delta between the high and low value
        /// </summary>
        /// <returns>delta of high and low</returns>
        public T GetSpan()
        {
            object objHigh = High;
            object objLow = Low;

            // As we cannot implicitely convert from T to the value types and reversely,                        // TODO: Check if this actually works!
            // we'll use the 'object' class as middle layer
            if (typeof(T) == typeof(float))
                return (T)(object)((float)objHigh - (float)objLow);

            else if (typeof(T) == typeof(double))
                return (T)(object)((double)objHigh - (double)objLow);

            else if (typeof(T) == typeof(BigDecimal))
                return (T)(object)((BigDecimal)objHigh - (BigDecimal)objLow);

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
            if (Low.CompareTo(low) <= 0 && High.CompareTo(high) >= 0)
                return true;

            return false;
        }

        /// <summary>
        /// Determines all points that intersect with the current <see cref="Range{T}"/>
        /// </summary>
        /// <param name="points">The points to check</param>
        /// <returns>All points that intersect the current <see cref="Range{T}"/></returns>
        public HashSet<T> Intersection(ICollection<T> points)
        {
            if (points == null)
                throw new System.ArgumentNullException(nameof(points));

            HashSet<T> intersections = new HashSet<T>();
            foreach (var point in points)
            {
                if (!intersections.Contains(point) && this.Contains(point))
                    intersections.Add(point);
            }

            return intersections;
        }

        /// <summary>
        /// Determines whether point intersects with the current range ( same as 'Contains'-method )
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True on intersection</returns>
        public bool Intersects(T point) => Contains(point);

        /// <summary>
        /// Determines whether any of the specified points intersect with the current <see cref="Range{T}"/>
        /// </summary>
        /// <param name="points">The points to check</param>
        /// <returns>True if any point is determined to intersect with the current <see cref="Range{T}"/></returns>
        public bool Intersects(ICollection<T> points)
        {
            if (points == null)
                throw new System.ArgumentNullException(nameof(points));

            foreach (var point in points)
            {
                if (this.Contains(point))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Range{T}"/> intersects with the current instance 
        /// </summary>
        /// <param name="range">The <see cref="Range{T}"/></param> to check
        /// <returns>Returns true on detected intersection</returns>
        public bool Intersects(Range<T> range)
        {
            // Is the higher bound of the range smaller than the lower bound of the current instance? (and vise versa)
            if (range.High.CompareTo(this.Low) < 0 || range.Low.CompareTo(this.High) > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Compares the high and low values of the specified range for equality
        /// </summary>
        /// <param name="obj">Object that needs to be of type <see cref="Range{T}"/> to check for equality, otherwise result will be false</param>
        /// <returns>True on equality</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj == null)
                return false;

            if (obj is Range<T> rng)
                return this.Low.Equals(rng.Low) && this.High.Equals(rng.High);

            return false;
        }

        /// <summary>
        /// The default hash function of <see cref="Range{T}"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = 31;
            result = (int)(17L * result * Low.GetHashCode());
            result = (int)(17L * result * High.GetHashCode());
            return result;
        }

        public override string ToString() => $"{Low} -> {High}";
    }
}
