using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManager.Application.Services
{
    public interface ITaskStatusUpdater
    {
        Task UpdateStatus();
        void SetTask(SaasTaskBase task);
    }

    public interface ITaskStatusUpdater<T> : ITaskStatusUpdater where T : SaasTaskBase
    {
        void SetTask(T task);
    }
}