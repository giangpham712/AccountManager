using System.Threading.Tasks;

namespace AccountManager.Application.Services
{
    public interface ILibraryFileService
    {
        Task<bool> FileExists(string fileUrl);
    }
}