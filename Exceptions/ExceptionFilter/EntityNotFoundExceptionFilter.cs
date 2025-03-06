using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace purpuraMain.Exceptions.ExceptionFilter;


public class EntityNotFoundExceptionFilter : IActionFilter, IOrderedFilter
{
      public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is EntityNotFoundException entityNotFoundException)
        {
            context.Result = new ObjectResult(entityNotFoundException.Value)
            {
                StatusCode = entityNotFoundException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}