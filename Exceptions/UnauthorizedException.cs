namespace purpuraMain.Exceptions;


public class UnauthorizedException : Exception
{
  public UnauthorizedException(int statusCode, object? value = null) =>
    (Value, StatusCode) = (value, statusCode);



    public object? Value{get ;}
    public int StatusCode {get;}
}