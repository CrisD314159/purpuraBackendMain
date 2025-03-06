using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace purpuraMain.Controllers;


[ApiController]
public class ExceptionHandler : ControllerBase
{

  [Route("/error-development")]
  [ApiExplorerSettings(IgnoreApi = true)]
  [AllowAnonymous]
  public IActionResult ErrorHandlerDevelopment(
    [FromServices] IHostEnvironment hostEnvironment
  )
  {
     if (!hostEnvironment.IsDevelopment())
    {
        return NotFound();
    }

    var exceptionHandlerFeature =
        HttpContext.Features.Get<IExceptionHandlerFeature>()!;

    return Problem(
        detail: exceptionHandlerFeature.Error.StackTrace,
        title: exceptionHandlerFeature.Error.Message);

  }

  [Route("/error")]
  [ApiExplorerSettings(IgnoreApi = true)]
public IActionResult HandleError() =>
    Problem();




}