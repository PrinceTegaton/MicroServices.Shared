using System;

namespace MicroServices
{
    public static class GuidHelper
    {
        public static bool IsValid(this Guid value)
        {
            if (value == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public static bool IsValid(this Guid? value)
        {
            if (value == null)
            {
                return false;
            }

            if (value == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public static bool IsValidGuid(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return Guid.TryParse(value, out _);
        }
    }
}