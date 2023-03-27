using System.Threading.Tasks;

namespace AccountManager.Application.Auth
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(string id, string userName, string[] permissions);
    }
}