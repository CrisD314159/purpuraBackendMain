namespace purpuraMain.Exceptions;

public class SessionExpiredException : Exception
{
    public SessionExpiredException(string message) : base(message)
    {
    }
}