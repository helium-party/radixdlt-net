using System;

namespace HeliumParty.RadixDLT
{
    /// <summary>
    /// Provides additional functionality when working with generic types
    /// </summary>
    public static class Generics
    {
        // TODO: Unit test

        /// <summary>
        /// Checks whether the specified type inherits or is an instance of the specified compare type.
        /// <para>
        /// Typical type checks don't work on generic classes (like <see cref="System.IComparable{T}"/>) as 
        /// they always require the generic type T to be specified.
        /// </para>
        /// <para>
        /// In other words:
        /// This basicly allows us to check whether a certain instance of the specified 'toCheck' type
        /// may be cast to the 'toCompare' type without any exceptions.
        /// </para>
        /// </summary>
        /// <param name="toCheck">The type we want to check</param>
        /// <param name="toCompareTo">What the specified type to check must inherit</param>
        /// <returns>Whether the type to check inherits or is an instance of the compare type</returns>
        public static bool InheritsOrIsInstance(this Type toCheck, Type toCompareTo)
        {
            Type current;
            while (toCheck != null && toCheck != typeof(object))
            {
                // For generics, we need to use the generic type definition ( other one would fail )
                current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;

                if (current == toCompareTo || InheritsOrIsInstanceInterface(current, toCompareTo))
                    return true;

                toCheck = toCheck.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified type is connected to the interface compare type
        /// </summary>
        /// <param name="toCheck">Type to check for inheritance of interface</param>
        /// <param name="compareTo">The interface</param>
        /// <returns>Whether type inherits interface</returns>
        private static bool InheritsOrIsInstanceInterface(Type toCheck, Type compareTo)
        {
            // The whole check only makes sense if the compare to type is an interface itself
            if (compareTo.IsInterface)
            {
                // GetInterfaces provides us all interfaces the type includes, so no need to
                // itterate into base type
                var interfacer = toCheck.GetInterfaces();

                foreach (var inter in interfacer)
                {
                    // For generics, we have to use the generic type definition
                    var tmp = inter.IsGenericType ? inter.GetGenericTypeDefinition() : inter;
                    if (tmp == compareTo)
                        return true;
                }
            }

            return false;
        }
    }
}
