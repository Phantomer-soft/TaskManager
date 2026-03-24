namespace TaskManager.GlobalExceptionHandling;

public class UserUpdatePasswordException : BaseExceptionHandler
{
    public UserUpdatePasswordException(
        string message,int statusCode) : base(message,statusCode)
    {
        
    }
}