using System;
using System.Globalization;

namespace Helpers
{
    public static class Converters
    {
        public static string ConvertToTitleCase(string value)
        {
            if (value == null) return null;
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(value.ToLower().Trim());
        }
        public static DateTimeOffset RoundToMultipleOfSix(DateTimeOffset originalTime)
        {
            int roundedMinutes = (int)Math.Ceiling(originalTime.Minute / 6.0) * 6;

            int finalMinutes = roundedMinutes < 60 ? roundedMinutes : 0;

            DateTimeOffset convertedTime = new DateTimeOffset(
                originalTime.Year, originalTime.Month, originalTime.Day,
                originalTime.Hour, finalMinutes, 0, originalTime.Offset);

            return convertedTime;
        }
        private static bool IsValidLatitude(decimal? latitude)
        {
            if (latitude == null) return false;
            return latitude >= -90 && latitude <= 90;
        }

        private static bool IsValidLongitude(decimal? longitude)
        {
            if (longitude == null) return false;
            return longitude >= -180 && longitude <= 180;
        }

        private static bool IsValidCoordinates(decimal? latitude, decimal? longitude)
        {
            return IsValidLatitude(latitude) && IsValidLongitude(longitude);
        }
        public static decimal? DMSToDecimalDegrees(int? degrees, int? minutes, int? seconds)
        {

            if (degrees == null || minutes == null || seconds == null) return null;

            return Math.Round((decimal)(degrees.Value + (minutes.Value / 60.0) + (seconds.Value / 3600.0)), 6);

        }
        public static decimal[] DMSToDecimalDegrees(int? latDegrees, int? latMinutes, int? latSeconds, int? lngDegrees, int? lngMinutes, int? lngSeconds)
        {
            decimal? lat = DMSToDecimalDegrees(latDegrees, latMinutes, latSeconds);
            decimal? lng = DMSToDecimalDegrees(lngDegrees, lngMinutes, lngSeconds);
            if (!IsValidCoordinates(lat, lng))
            {
                return null;
            }
            return new decimal[] { lat.Value, lng.Value };
        }
    }
}
