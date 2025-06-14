using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;


public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{

  private readonly ILogger<GlobalExceptionFilter> _logger = logger;


  private static readonly Dictionary<Type, int> ExceptionStatusCodes = new()
    {
      {typeof(BadRequestException), StatusCodes.Status400BadRequest},
      {typeof(EntityNotFoundException), StatusCodes.Status404NotFound},
      {typeof(NotVerifiedException), StatusCodes.Status401Unauthorized},
      {typeof(NullFieldException), StatusCodes.Status400BadRequest},
      {typeof(SessionExpiredException), StatusCodes.Status401Unauthorized},
      {typeof(UnauthorizedException), StatusCodes.Status401Unauthorized},
      {typeof(InternalServerException), StatusCodes.Status500InternalServerError}
    };
  public void OnException(ExceptionContext context)
  {
    var exceptionType = context.Exception.GetType();

    if(ExceptionStatusCodes.TryGetValue(exceptionType, out int statusCodes))
    {
      context.Result = new ObjectResult(new {message = "An unexpected error occured", success = false})
      {
        StatusCode = statusCodes
      };
    }
    else{
        _logger.LogError(context.Exception, "Unhandled exception");
        context.Result = new ObjectResult(new { message = "An unexpected error occured", success = false })
        {
          StatusCode = StatusCodes.Status500InternalServerError
        };
    }
  }
}