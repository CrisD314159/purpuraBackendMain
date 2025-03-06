namespace purpuraMain.Exceptions;

public class NotVerifiedException : Exception
{
  public NotVerifiedException(int statusCode, object? value = null) =>
    (Value, StatusCode) = (value, statusCode);



    public object? Value{get ;}
    public int StatusCode {get;}
}