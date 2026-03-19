namespace TaskManager;

public static class Functions
{
    public static string Hash(string unhashedText)
    {
        return BCrypt.Net.BCrypt.HashPassword(unhashedText);
    }
    public static bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
}