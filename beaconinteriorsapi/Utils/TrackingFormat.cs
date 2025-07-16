namespace beaconinteriorsapi.Utils
{
    public static class TrackingFormat
    {
        public static string ToTrackingFormat(this long number)
        {
            // Pad the number to 16 digits
            string padded = number.ToString().PadLeft(16, '0');

            // Format as 0000-0000-0000-0000
            return $"{padded[..4]}-{padded[4..8]}-{padded[8..12]}-{padded[12..16]}";
        }

    }
}
