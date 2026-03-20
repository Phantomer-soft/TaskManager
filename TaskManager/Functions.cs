namespace TaskManager;

public static class Functions
{
    public static string Hash(string UnhashedText)
    {
        return BCrypt.Net.BCrypt.HashPassword(UnhashedText);
    }
    public static bool Verify(string Password, string HashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(Password, HashedPassword);
    }
    
}