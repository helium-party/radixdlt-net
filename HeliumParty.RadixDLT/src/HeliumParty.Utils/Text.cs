using System;
using System.Globalization;
using System.Text;

namespace HeliumParty.Utils
{
    /// <summary>
    /// Provides functionality for string manipulation
    /// </summary>
    public static class Text
    {
        /// <summary>
        /// Supported cases
        /// </summary>
        public enum NamingCase
        {
            /// <summary>
            /// Only capital letters, words separated by underscores
            /// </summary>
            UPPER_CASE,

            /// <summary>
            /// Capital letters at the start of every word, words are not separated
            /// </summary>
            UpperCamelCase,

            /// <summary>
            /// Capital letters, except for the first, at the start of every word, words are not separated. 
            /// </summary>
            lowerCamelCase,

            /// <summary>
            /// ONly small letters, words are separated by underscores
            /// </summary>
            snake_case,
        }

        /// <summary>
        /// Wrapper to convert text from <see cref="NamingCase.UPPER_CASE"/> to <see cref="NamingCase.UpperCamelCase"/>
        /// </summary>
        /// <param name="input">The text to convert</param>
        /// <returns>The converted text</returns>
        public static string FromUpperToUpperCamelCase(this string input) =>
            ChangeCase(input, NamingCase.UPPER_CASE, NamingCase.UpperCamelCase);

        /// <summary>
        /// Wrapper to convert text from <see cref="NamingCase.UpperCamelCase"/> to <see cref="NamingCase.UPPER_CASE"/>
        /// </summary>
        /// <param name="input">The text to convert</param>
        /// <returns>The converted text</returns>
        public static string FromUpperCamelToUpperCase(this string input) =>
            ChangeCase(input, NamingCase.UpperCamelCase, NamingCase.UPPER_CASE);

        /// <summary>
        /// Changes the casing of a string to the specified <paramref name="targetCase"/>
        /// </summary>
        /// <param name="input">The string to manipulate</param>
        /// <param name="originalCase">The original naming case of the string</param>
        /// <param name="targetCase">The new case for the string</param>
        /// <returns>The string with the changed case</returns>
        public static string ChangeCase(this string input, NamingCase originalCase, NamingCase targetCase)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input == string.Empty)
                return string.Empty;

            if (originalCase == targetCase)
                return input;

            switch (targetCase)
            {
                case NamingCase.UPPER_CASE:
                    return ToUpperCase(input, originalCase);
                case NamingCase.UpperCamelCase:
                    return ToUpperCamelCase(input, originalCase);
                case NamingCase.lowerCamelCase:
                    return ToLowerCamelCase(input, originalCase);
                case NamingCase.snake_case:
                    return ToSnakeCase(input, originalCase);
                default:
                    break;
            }

            throw new NotImplementedException($"Case changing from {originalCase} to {targetCase} is not implemented.");
        }

        internal static string ToUpperCamelCase(string input, NamingCase originalCase)
        {
            if (originalCase == NamingCase.lowerCamelCase)
                return input.Length == 1
                    ? input.ToUpperInvariant()
                    : Char.ToUpperInvariant(input[0]) + input.Substring(1);

            else if (originalCase == NamingCase.UPPER_CASE)
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Replace('_', ' ').ToLowerInvariant())
                    .Replace(" ", string.Empty);

            else if (originalCase == NamingCase.snake_case)
                return ToUpperCamelCase(input.ToUpperInvariant(), NamingCase.UPPER_CASE);

            else
                return input;
        }

        internal static string ToLowerCamelCase(string input, NamingCase originalCase)
        {
            if (originalCase == NamingCase.UpperCamelCase)
                return input.Length == 1
                    ? input.ToLowerInvariant()
                    : Char.ToLowerInvariant(input[0]) + input.Substring(1);

            else if (originalCase == NamingCase.UPPER_CASE)
                return ToLowerCamelCase(
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.Replace('_', ' ').ToLowerInvariant())
                        .Replace(" ", string.Empty),
                    NamingCase.UpperCamelCase);

            else if (originalCase == NamingCase.snake_case)
                return ToLowerCamelCase(input.ToUpperInvariant(), NamingCase.UPPER_CASE);

            else
                return input;
        }

        internal static string ToUpperCase(string input, NamingCase originalCase)
        {
            if (originalCase == NamingCase.snake_case)
                return input.ToUpperInvariant();

            else if (originalCase == NamingCase.UpperCamelCase
                || originalCase == NamingCase.lowerCamelCase)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < input.Length; i++)
                {
                    // For each capital letter, add a '_' previous to that
                    if (Char.IsUpper(input[i]) && !(i == 0))
                        sb.Append($"_{input[i]}");
                    else
                        sb.Append(input[i]);
                }

                return sb.ToString().ToUpperInvariant();
            }
            else
                return input;
        }

        internal static string ToSnakeCase(string input, NamingCase originalCase)
        {
            if (originalCase == NamingCase.UPPER_CASE)
                return input.ToLowerInvariant();

            else if (originalCase == NamingCase.UpperCamelCase
                || originalCase == NamingCase.lowerCamelCase)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < input.Length; i++)
                {
                    // For each capital letter, add a '_' previous to that
                    if (Char.IsUpper(input[i]) && !(i == 0))
                        sb.Append($"_{input[i]}");
                    else
                        sb.Append(input[i]);
                }

                return sb.ToString().ToLowerInvariant();
            }
            else
                return input;
        }
    }
}
