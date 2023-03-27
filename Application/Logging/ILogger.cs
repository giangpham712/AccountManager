namespace AccountManager.Application.Logging
{
    public interface ILogger
    {
        void LogError();
        void LogError(string s);
    }
}