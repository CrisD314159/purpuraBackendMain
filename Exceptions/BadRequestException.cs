namespace purpuraMain.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(int statusCode, object? value = null) =>
    (Value, StatusCode) = (value, statusCode);



    public object? Value{get ;}
    public int StatusCode {get;}
}