namespace TaskManager.GlobalExceptionHandling;

public class UserLoginException : BaseExceptionHandler
{
    // giris yapmamis kullanicilar icin kullanilacak 
    public UserLoginException(string message) 
        : base(message, 401)
    { }

    
}