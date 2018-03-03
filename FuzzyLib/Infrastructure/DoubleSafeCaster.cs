namespace FuzzyLib.Infrastructure
{
    static class DoubleSafeCaster
    {
        public static double Convert(object value)
        {
            double tryvalue;
            if (double.TryParse(value.ToString(), out tryvalue))
            {
                return tryvalue;
            }

            return 0.0;
        }
    }
}
