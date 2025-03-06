namespace purpuraMain.Exceptions;

public class EntityNotFoundException : Exception
{
     public EntityNotFoundException(int statusCode, object? value = null) =>
    (Value, StatusCode) = (value, statusCode);



    public object? Value{get ;}
    public int StatusCode {get;}
}