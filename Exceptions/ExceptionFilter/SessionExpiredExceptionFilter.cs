using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;


public class SessionExpiredExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is SessionExpiredException sessionExpiredException)
        {
            context.Result = new ObjectResult(sessionExpiredException.Value)
            {
                StatusCode = sessionExpiredException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}