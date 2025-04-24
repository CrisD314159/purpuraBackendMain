namespace purpuraMain.Exceptions;
public class NullFieldException(string message) : Exception
{

    public override string Message{get ;} = message;
    public int StatusCode {get;} = 400;
}