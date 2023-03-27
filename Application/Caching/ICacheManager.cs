namespace AccountManager.Application.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);

        void Set(string key, object data, int cacheTime, bool sliding = false);

        bool IsSet(string key);

        void Remove(string key);

        void Clear();
    }
}