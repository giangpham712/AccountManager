using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.Application.Tasks
{
    public interface ITaskManager
    {
        void Init();
        Task QueueTaskAsync(ITask task);
        void DeleteTask(ITask task);

        List<ITask> ListTasksByMachine(long machineId);
    }
}