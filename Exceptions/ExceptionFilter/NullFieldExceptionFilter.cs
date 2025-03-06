using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;
public class NullFieldExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is NullFieldException nullFieldException)
        {
            context.Result = new ObjectResult(nullFieldException.Value)
            {
                StatusCode = nullFieldException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}