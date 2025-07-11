namespace beaconinteriorsapi.Response
{
    public class ErrorResponse
    {
        public  string? Message { get; set; } = null;
        public  IDictionary<string, string[]>? Errors { get; set; } = null;
        public ErrorResponse()
        {
            
        }
        public ErrorResponse(string message)
        {
                Message = message;
        }
        public ErrorResponse(IDictionary<string, string[]>? errors)
        {
            Message = "Bad Request";
            Errors = errors;  
        }
        public static ErrorResponse Create(string? message=null, IDictionary<string, string[]>? errors=null) {
            if ( errors!=null) { 
                return new ErrorResponse(errors);
            }

            return new ErrorResponse(message!);
        }
    }
}
