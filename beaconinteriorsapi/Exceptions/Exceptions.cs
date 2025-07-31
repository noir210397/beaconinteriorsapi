using beaconinteriorsapi.Response;

namespace beaconinteriorsapi.Exceptions
{
    public abstract class BaseException:Exception
    {
        public  ErrorResponse Response { get;  }
        public  string LogMessage {  get;  }
        protected BaseException(ErrorResponse response,string? logMessage=null)
        {
           Response = response;
            LogMessage = logMessage??response.Message!;
        }

    }
    public class NotFoundException :BaseException {
        public NotFoundException(ErrorResponse response,string? logMessage):base(response,logMessage) {
        }
    }
    public class BadRequestException : BaseException {
        public object? Result;
        public BadRequestException(ErrorResponse response, string? logMessage) : base(response, logMessage)
        {
        }

    }
    public class ServerErrorException : BaseException
    {
        public ServerErrorException(ErrorResponse response, string? logMessage) : base(response, logMessage)
        {
        }
    }
    public class UnauthorizizedException : BaseException
    {
        public UnauthorizizedException(ErrorResponse response, string? logMessage) : base(response, logMessage)
        {
        }
    }

    public static class ExceptionHelpers
        {
            public static void ThrowNotFound(string? message, IDictionary<string, string[]>? errors= null,  string? logMessage= null)
                => throw new NotFoundException(ErrorResponse.Create(message,errors),logMessage);

            public static void ThrowBadRequest(string? message, IDictionary<string, string[]>? errors = null, string? logMessage = null)
                => throw new BadRequestException(ErrorResponse.Create(message, errors), logMessage);

            public static void ThrowServerError( string? logMessage = null)
                => throw new ServerErrorException(ErrorResponse.Create("internal server error"), logMessage);
            public static void ThrowUnauthorizedError(string message ,string? logMessage = null)
                => throw new UnauthorizizedException(ErrorResponse.Create(message), logMessage);
    }
    

}
