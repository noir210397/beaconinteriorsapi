using Microsoft.AspNetCore.Mvc;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;

namespace beaconinteriorsapi.Controllers.Base
{
    public abstract class MyBaseController:ControllerBase
    {
        protected abstract string ResourceName { get; }

        protected void ValidateString(string value, string message = "unable to access resource as invalid value was provided ")
            {
                if (string.IsNullOrEmpty(value)) { ThrowBadRequest(message?? $"unable to access {ResourceName} as invalid value was provided",null,message); }
            }
            protected Guid ToGuidOrThrowBadRequestError(string id)
            {
                if (!Guid.TryParse(id, out var guid))
                {
                    ThrowBadRequest($"unable to get {ResourceName} as invalid ID was provided",null, $"Invalid {ResourceName} ID");
                }
                return guid;
            }
        protected int ValidatePageNumber(int value) {
            if ( value == 0)
            {
                return 1;
            }
            return value;
        }
    }
}
