using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;



public class UnauthorizedExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is UnauthorizedException unauthorizedException)
        {
            context.Result = new ObjectResult(unauthorizedException.Value)
            {
                StatusCode = unauthorizedException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}