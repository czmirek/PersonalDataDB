namespace PersonalDataDB
{
    using System;
    using System.Linq;

    internal static class InternalExtensions
    {
        internal static bool IsEmptyOrWhiteSpace(this string str) => str == String.Empty || str.Any(x => Char.IsWhiteSpace(x));
    }
}