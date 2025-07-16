namespace beaconinteriorsapi.Utils
{
    public static class MoneyExtensions
    {
        public static long ToPence(this decimal amount)
        {
            return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
        }

        public static long ToPence(this double amount)
        {
            return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
        }
    }

}
