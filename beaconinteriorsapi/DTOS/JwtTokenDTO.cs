﻿namespace beaconinteriorsapi.DTOS
{
    public class JwtTokenDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
