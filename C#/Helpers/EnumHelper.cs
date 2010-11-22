using System;
using System.Linq;

namespace PO.Common.Helpers
{
    public static class EnumHelper
    {
        public static T ToEnum<T>(this string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, false);
        }

        public static bool TryParseEnum<T>(this string valueToParse, out T returnValue)
        {
            if (Enum.GetNames(typeof(T)).Contains(valueToParse))
            {
                returnValue = ToEnum<T>(valueToParse);
                return true;
            }
            returnValue = default(T);
            return false;
        }
    }
}
