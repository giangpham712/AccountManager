using System;
using System.Threading.Tasks;

namespace AccountManager.Application.Tasks
{
    public enum TaskStatus
    {
        New,
        Queued,
        Running,
        Failed,
        Completed
    }

    public interface ITask
    {
        Guid Id { get; set; }
        TaskType Type { get; }
        TaskStatus Status { get; set; }
        string Progress { get; set; }
        string Error { get; set; }
        DateTimeOffset QueuedAt { get; set; }
        DateTimeOffset StartedAt { get; set; }
        DateTimeOffset? FinishedAt { get; set; }
        long AccountId { get; set; }
        long? MachineId { get; set; }

        string User { get; set; }

        Task ExecuteAsync(IServiceProvider serviceProvider);
    }

    public interface ITaskArgs
    {
    }

    public abstract class AMTask<TTaskArgs> : ITask where TTaskArgs : ITaskArgs
    {
        protected TTaskArgs TaskArgs;

        protected AMTask(TTaskArgs taskArgs)
        {
            TaskArgs = taskArgs;
        }

        public int Timeout { get; set; }
        public Guid Id { get; set; }

        public abstract TaskType Type { get; }
        public TaskStatus Status { get; set; }
        public string Progress { get; set; }
        public string Error { get; set; }
        public DateTimeOffset QueuedAt { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public long AccountId { get; set; }
        public long? MachineId { get; set; }

        public string User { get; set; }

        public abstract Task ExecuteAsync(IServiceProvider serviceProvider);
    }

    public abstract class MachineTaskBase<TTaskArgs> : AMTask<TTaskArgs> where TTaskArgs : ITaskArgs
    {
        protected MachineTaskBase(TTaskArgs taskArgs) : base(taskArgs)
        {
        }
    }
}