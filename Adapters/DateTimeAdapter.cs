using System;

namespace MicroServices.Shared.Adapters
{
    public interface IDateTimeAdapter
    {
        DateTime Now { get; }
        DateTime GetUtcNow(int? offsetInHours = null);
    }

    public class DateTimeAdapter : IDateTimeAdapter
    {
        private static int UtcOffset;

        public DateTimeAdapter(int utcOffsetInHours = 0)
        {
            UtcOffset = utcOffsetInHours;
        }

        public DateTime Now { get => DateTime.UtcNow.AddHours(UtcOffset); }

        public DateTime GetUtcNow(int? utcOffset = null)
        {
            DateTime dt = DateTime.UtcNow;
            if (UtcOffset > 0 || utcOffset.HasValue)
            {
                dt = dt.AddHours(utcOffset ?? UtcOffset);
            }

            return dt;
        }
    }
}