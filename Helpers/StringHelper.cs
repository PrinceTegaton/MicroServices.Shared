using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MicroServices
{
    public static class StringHelper
    {
        public static string BlankToNull(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string Trim(this string value, int limit)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (value.Length > limit) return value[..limit];
            if (limit > value.Length) return value;

            return value;
        }

        public static string ToSnakeCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var startUnderscores = Regex.Match(value, @"^_+");
            return startUnderscores + Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }


        public static string ToMoney(this decimal value, Currency? currency = Currency.Naira, int decimals = 2, bool showZeroFloatingPoint = true)
        {
            string text = value.ToString($"N{decimals}");

            if (!showZeroFloatingPoint && text.Contains('.'))
            {
                string[] a = text.Split(".");
                if (a[1] == "00")
                {
                    text = a[0];
                }
            }

            if (currency == null)
            {
                return text;
            }
            else
            {
                return $"{GetCurrencySymbol(currency.Value)}{text}";
            }
        }

        public static string ToMoney(this decimal? value, Currency? currency = Currency.Naira, int decimals = 2, bool showZeroFloatingPoint = true)
        {
            if (!value.HasValue)
            {
                return null;
            }

            return ToMoney(value.Value, currency, decimals, showZeroFloatingPoint);
        }

        public static string GetCurrencySymbol(Currency currency)
        {
            string symbol = string.Empty;

            switch (currency)
            {
                case Currency.Naira:
                    symbol = "₦";
                    break;
                case Currency.Dollar:
                    symbol = "$";
                    break;
                case Currency.Pound:
                    symbol = "£";
                    break;
                case Currency.Euro:
                    symbol = "€";
                    break;
                case Currency.Yen:
                    symbol = "¥";
                    break;
                default:
                    break;
            }

            return symbol;
        }

        public static string ToWords(this Enum value)
        {
            string str = value.ToString();
            //string s2 = System.Text.RegularExpressions.Regex.Replace(str, "(\\B[A-Z])", " ").Replace("_", "");

            string formatted = string.Empty;
            char previous = str[0];
            foreach (var i in str)
            {
                if (char.IsUpper(i) && char.IsLower(previous))
                {
                    formatted += $" {i}";
                }
                else
                {
                    formatted += i;
                }

                previous = i;
            }

            return formatted.Replace("_", " ")
                            .Replace(" And ", " and ")
                            .Replace(" For ", " for ")
                            .Replace(" Or ", " or ");
        }




        public static bool IsValid(this long value)
        {
            if (value <= 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsValid(this long? value)
        {
            if (value == null)
            {
                return false;
            }

            if (value <= 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsValidLong(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return long.TryParse(value, out _);
        }

        private static long lastTimeStamp = DateTime.Now.Ticks;
        public static string GenerateTransactionReference()
        {
            long original, newValue;

            do
            {
                original = lastTimeStamp;
                long now = DateTime.Now.Ticks;
                newValue = Math.Max(now, original + 1);
            } while (System.Threading.Interlocked.CompareExchange(ref lastTimeStamp, newValue, original) != original);

            return newValue.ToString();
        }
    }
}