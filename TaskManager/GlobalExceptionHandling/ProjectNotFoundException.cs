namespace TaskManager.GlobalExceptionHandling;

public class ProjectNotFoundException : BaseExceptionHandler
{
    public ProjectNotFoundException(string message) : base(message,404)
    {
    }
}