using System;
using System.Linq;

namespace HeliumParty.RadixDLT.Primitives
{
    public static class Arrays
    {
        public static void Fill<T>(ref T[] arrayToFill, int startIdx, T value)
        {
            if (arrayToFill == null)
                throw new ArgumentNullException(nameof(arrayToFill));

            if (startIdx >= arrayToFill.Length)
                throw new ArgumentOutOfRangeException(nameof(startIdx) + " is greater than the array size!");

            Fill<T>(ref arrayToFill, startIdx, arrayToFill.Length, value);
        }

        /// <summary>
        /// Sets the values to the specified for a specified range in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayToFill"></param>
        /// <param name="firstIdx">The index of the first element that should be set (inclusive)</param>
        /// <param name="lastIdx">The index of the last element that should be set (inclusive), 
        /// if greater than array size, array size will be used instead</param>
        /// <param name="value"></param>
        public static void Fill<T>(ref T[] arrayToFill, int firstIdx, int lastIdx, T value)
        {
            if (arrayToFill == null)
                throw new ArgumentNullException(nameof(arrayToFill));

            if (firstIdx >= arrayToFill.Length)
                throw new ArgumentOutOfRangeException(nameof(firstIdx) + " is greater than the array size!");

            if (firstIdx > lastIdx)
                throw new ArgumentException(nameof(firstIdx) + " is greater than " + nameof(lastIdx) + "!");

            // Prevent exception by limiting the max index
            var maxIdx = (lastIdx < arrayToFill.Length) ? lastIdx : arrayToFill.Length - 1;
            for (int i = firstIdx; i <= maxIdx; i++)
            {
                arrayToFill[i] = value;
            }
        }

        public static void Fill<T>(this T[] array, int start, int end, T value)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (start < 0 || start >= end)
                    throw new ArgumentOutOfRangeException("fromIndex");
            if (end >= array.Length)
                throw new ArgumentOutOfRangeException("toIndex");
            for (int i = start; i < end; i++)
                array[i] = value;
        }

        /// <summary>
        /// Extends the array to the specified size by filling in the specified value 
        /// either at the beginning or the ending of the current values in the array
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="arrayToExtend">Array to extend</param>
        /// <param name="size">Size the array should be extended to</param>
        /// <param name="extendAtBeginning">On true, the size will be extended by 'adding' indexes 
        /// previous the current array values</param>
        /// <param name="value">Value that should be used when extending the array</param>
        /// <returns>The extended array</returns>
        public static T[] Extend<T>(T[] arrayToExtend, int size, bool extendAtBeginning, T value)
        {
            // TODO: Exception handling

            if (arrayToExtend.Length >= size)
                return arrayToExtend;

            var extendedArray = new T[size];
            var bytesToFill = size - arrayToExtend.Length;

            if (extendAtBeginning)
            {
                Fill<T>(ref extendedArray, 0, bytesToFill, value);
                System.Array.Copy(arrayToExtend, 0, extendedArray, bytesToFill, arrayToExtend.Length);
            }
            else
            {
                Fill<T>(ref extendedArray, bytesToFill, extendedArray.Length - 1, value);
                System.Array.Copy(arrayToExtend, 0, extendedArray, 0, arrayToExtend.Length);
            }

            return extendedArray;
        }

        /// <summary>
        /// Copies a specified range of an array into a new array. 
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="original">the array to copy from</param>
        /// <param name="from">the starting index</param>
        /// <param name="to">the final index</param>
        /// <returns>a copy of a specified range of the original array</returns>
        public static T[] CopyOfRange<T>(T[] original, int from, int to)
        {
            var length = to - from;

            if (length < 0)
                throw new ArgumentException(nameof(from) + " has to be < than " + nameof(to));

            var copy = new T[length];
            Array.Copy(original, from, copy, 0, Math.Min(length, original.Length - from));
            return copy;
        }

        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var result = new T[arrays.Sum(arr => arr.Length)];
            var offset = 0;
            foreach (var arr in arrays)
            {
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }

        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {
            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            if (start < 0)
                throw new ArgumentException(nameof(start));


            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start = 0)
        {
            return SubArray(arr, start, arr.Length - start);
        }
    }
}