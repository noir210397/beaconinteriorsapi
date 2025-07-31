using beaconinteriorsapi.Exceptions;
using beaconinteriorsapi.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace beaconinteriorsapi.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;   
        }
        public void OnException(ExceptionContext context)
        {

            switch (context.Exception)
            {
                case NotFoundException ex:
                    _logger.LogWarning(ex, ex.LogMessage??ex.Message);
                    context.Result = new NotFoundObjectResult(ex.Response);
                    break;

                case BadRequestException ex:
                    _logger.LogInformation(ex, ex.LogMessage ?? ex.Message);
                    context.Result = new BadRequestObjectResult(ex.Response);
                    break;

                case UnauthorizizedException ex:
                    _logger.LogError(ex, ex.LogMessage ?? ex.Message);
                    context.Result = new UnauthorizedObjectResult(ex.Response);
                    break;

                case ServerErrorException ex:
                    _logger.LogError(ex, ex.LogMessage ?? ex.Message);
                    context.Result = new ObjectResult(ex.Response)
                    {
                        StatusCode = 500
                    };
                    break;

                default:
                    _logger.LogError(context.Exception, "Unhandled Exception Message:{Message}",context.Exception.Message);
                    context.Result = new ObjectResult(ErrorResponse.Create("Internal Server Error Currently Unknown"))
                    {
                        StatusCode = 500
                    };
                    break;
            }

            context.ExceptionHandled = true;
        }

    }
}
