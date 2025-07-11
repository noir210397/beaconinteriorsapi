namespace beaconinteriorsapi.Utils
{
    public static class GuidUtils
    {

        public static string ToGuidString(Guid guid)
        {
            return guid.ToString();
        }

        //public static Guid ToGuid(string input,string name )
        //{
        //    if (!Guid.TryParse(input, out var result))
        //    {
        //        throw new ArgumentException($"Invalid {name} Id ");
        //    }

        //    return result;
        //}
    }

}
