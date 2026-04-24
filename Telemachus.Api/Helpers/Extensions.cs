namespace Helpers
{
    public static class StringExtensions
    {
        public static double? ToDouble(this string value)
        {
            double doubleVal = 0;
            if (value != null)
            {
                double tempValue;
                if (double.TryParse(value, out tempValue))
                {
                    doubleVal = tempValue;
                }
            }
            return doubleVal;
        }
    }
}
