using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;

public class BadRequestExceptionFilter : IActionFilter, IOrderedFilter
{
      public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is BadRequestException badRequestException)
        {
            context.Result = new ObjectResult(badRequestException.Value)
            {
                StatusCode = badRequestException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}