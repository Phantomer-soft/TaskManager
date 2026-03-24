namespace TaskManager.GlobalExceptionHandling;

public class UserRegisterException : BaseExceptionHandler
{
    public UserRegisterException(string message) : base(message,422)
    {
        
    }
}