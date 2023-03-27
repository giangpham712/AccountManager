namespace AccountManager.Application.Auth
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}