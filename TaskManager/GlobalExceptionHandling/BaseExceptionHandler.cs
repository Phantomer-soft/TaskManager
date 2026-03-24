namespace TaskManager.GlobalExceptionHandling;

public class BaseExceptionHandler : Exception
{
    public int StatusCode { get; }

    public BaseExceptionHandler(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }
}