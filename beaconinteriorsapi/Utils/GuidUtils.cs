namespace beaconinteriorsapi.Utils
{
    public static class GuidUtils
    {

        public static string ToGuidString(Guid guid)
        {
            return guid.ToString();
        }

        public static bool ToGuid(string input)
        {
            if (!Guid.TryParse(input, out var result))
            {
                return false;
            }

            return true;
        }
    }

}
