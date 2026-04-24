using System;

namespace Helpers
{
    public class CorrectionFactorModel
    {
        public double WeightCorrectionFactor { get; set; }
        public double VolumeCorrectionFactor { get; set; }
        public double GrossStandardVolume { get; set; }
        public double Weight { get; set; }
        public double Volume { get; set; }
    }
    public static class BunkeringTools
    {

        public static double GetDouble(string value)
        {
            double.TryParse(value, out double numericValue);
            return numericValue;
        }
        public static double GetDouble2(string value, int decimals = 3)
        {
            double.TryParse(value, out double numericValue);

            return GetDouble(numericValue.ToString($"F{decimals}"));
        }
        public static decimal GetDecimal(string value)
        {
            decimal.TryParse(value, out decimal numericValue);
            return numericValue;
        }

        private static double GetWeightCorrectionFactor(double fuelDensity)
        {
            return fuelDensity - 0.0011;
        }

        private static double GetVolumeCorrectionFactor(double fuelDensity, double tankTemperature)
        {
            double factor;

            if (fuelDensity < 0.8385)
            {
                factor = (594.5418 / Math.Pow(fuelDensity * 1000, 2)) * (tankTemperature - 15) *
                         (1 + 0.8 * (594.5418 / Math.Pow(fuelDensity * 1000, 2)) * (tankTemperature - 15));
            }
            else
            {
                factor = (186.9696 / Math.Pow(fuelDensity * 1000, 2) + 0.4862 / (fuelDensity * 1000)) * (tankTemperature - 15) *
                         (1 + 0.8 * (186.9696 / Math.Pow(fuelDensity * 1000, 2) + 0.4862 / (fuelDensity * 1000)) * (tankTemperature - 15));
            }

            return Math.Pow(Math.E, -factor);
        }
        public static CorrectionFactorModel GetWeightCorrectionFactor(string density, string temp, string volume)
        {
            var weightCorrectionFactor = GetWeightCorrectionFactor(GetDouble(density));
            var volumeCorrectionFactor = GetVolumeCorrectionFactor(GetDouble(density), GetDouble(temp));

            decimal grossStandardVolume = GetDecimal(volumeCorrectionFactor.ToString("F4")) * GetDecimal(volume);
            decimal weight = GetDecimal(weightCorrectionFactor.ToString("F4")) * grossStandardVolume;

            return new CorrectionFactorModel()
            {
                WeightCorrectionFactor = weightCorrectionFactor,
                VolumeCorrectionFactor = volumeCorrectionFactor,
                GrossStandardVolume = (double)grossStandardVolume,
                Volume = GetDouble(volume),
                Weight = (double)weight
            };
        }
        public static CorrectionFactorModel GetVolumeCorrectionFactor(string density, string temp, string weight)
        {
            var weightCorrectionFactor = GetWeightCorrectionFactor(GetDouble(density));
            var volumeCorrectionFactor = GetVolumeCorrectionFactor(GetDouble(density), GetDouble(temp));
            decimal grossStandardVolume = GetDecimal(weight) / GetDecimal(weightCorrectionFactor.ToString("F4"));
            decimal volume = grossStandardVolume / GetDecimal(volumeCorrectionFactor.ToString("F4"));
            return new CorrectionFactorModel()
            {
                WeightCorrectionFactor = weightCorrectionFactor,
                VolumeCorrectionFactor = volumeCorrectionFactor,
                GrossStandardVolume = (double)grossStandardVolume,
                Volume = (double)volume,
                Weight = GetDouble(weight)
            };
        }
    }

}
