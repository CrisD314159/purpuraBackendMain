namespace purpuraMain.Exceptions;

public class SessionExpiredException : Exception
{
  public SessionExpiredException(int statusCode, object? value = null) =>
    (Value, StatusCode) = (value, statusCode);



    public object? Value{get ;}
    public int StatusCode {get;}
}