namespace purpuraMain.Exceptions;

public class EntityNotFoundException(string message) : Exception
{
    public override string Message{get ;} = message;
    public int StatusCode {get;} = 401;
}