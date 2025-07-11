namespace purpuraMain.Exceptions;

public class BadRequestException (string message) : Exception
{

    public override string Message{get ;} = message;
    public int StatusCode {get;} = 400;
}