using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using Microsoft.AspNetCore.SignalR;

namespace AccountManager.Application.Tasks
{
    public class TaskHub : Hub<ITaskClient>
    {
        
    }

    public interface ITaskClient
    {
        Task TaskQueued(AMTaskDto task);
        Task TaskStarted(AMTaskDto task);
        Task TaskFinished(AMTaskDto task);
    }
}