using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Common.Extensions;

namespace AccountManager.Application.Services
{
    public class TaskTrackingService : ITaskTrackingService
    {
        private static int _updateStatusInverval = 10000;
        private readonly IDictionary<Guid, SaasTaskBase> _trackedTasks;
        private readonly IDictionary<Guid, ITaskStatusUpdater> _taskStatusUpdaters;

        public TaskTrackingService()
        {
            _trackedTasks = new ConcurrentDictionary<Guid, SaasTaskBase>();

            _taskStatusUpdaters = new ConcurrentDictionary<Guid, ITaskStatusUpdater>();
        }

        public void Init()
        {
            Task.Factory.StartNew(async () => await TrackTaskStatus());
        }

        private async Task TrackTaskStatus()
        {
            while (true)
            {
                await Task.Delay(_updateStatusInverval);
                foreach (var task in ActiveTasks)
                    try
                    {
                        var updaterType = typeof(ITaskStatusUpdater<>).MakeGenericType(task.GetType());
                        var updater = null as ITaskStatusUpdater;
                        if (updater == null) continue;
                        updater.SetTask(task);
                        await updater.UpdateStatus();
                    }
                    catch (Exception e)
                    {
                        //
                    }
            }
        }

        public IEnumerable<SaasTaskBase> ActiveTasks
        {
            get { return _trackedTasks.Values.Where(x => x.IsActive); }
        }

        public void Track(SaasTaskBase task)
        {
            if (!_trackedTasks.ContainsKey(task.Id))
                _trackedTasks.Add(task.Id, task);
        }

        public void Untrack(SaasTaskBase task)
        {
            Untrack(task.Id);
        }

        public void Untrack(Guid id)
        {
            if (!_trackedTasks.ContainsKey(id))
                _trackedTasks.Remove(id);
        }

        public IEnumerable<SaasTaskBase> GetActiveTasksForAccount(long accountId)
        {
            return _trackedTasks.Values.Where(x => x.AccountId == accountId && x.IsActive).OrderBy(x => x.StartedAt);
        }

        public IEnumerable<SaasTaskBase> GetActiveTasksForSite(long siteId)
        {
            return _trackedTasks.Values.Where(x => x.SiteId == siteId && x.IsActive).OrderBy(x => x.StartedAt);
        }

        public IEnumerable<SaasTaskBase> GetActiveTasksForMachine(long machineId)
        {
            return _trackedTasks.Values.Where(x => x.MachineId == machineId && x.IsActive).OrderBy(x => x.StartedAt);
        }
    }

    public interface ITaskTrackingService
    {
        void Init();

        void Track(SaasTaskBase task);
        void Untrack(SaasTaskBase task);
        void Untrack(Guid id);

        IEnumerable<SaasTaskBase> GetActiveTasksForAccount(long accountId);
        IEnumerable<SaasTaskBase> GetActiveTasksForSite(long siteId);
        IEnumerable<SaasTaskBase> GetActiveTasksForMachine(long machineId);
    }

    public abstract class SaasTaskBase
    {
        public Guid Id { get; private set; }
        public SaasTaskStatus Status { get; internal set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public DateTimeOffset StatusAt { get; internal set; }
        public string StatusDetail { get; internal set; }
        public int Progress { get; internal set; }

        public long? AccountId { get; set; }
        public long? SiteId { get; set; }
        public long? MachineId { get; set; }
        public abstract string Name { get; }

        protected SaasTaskBase()
        {
            Id = Guid.NewGuid();
            Status = SaasTaskStatus.Queued;
            StartedAt = DateTimeOffset.Now;
        }

        public bool IsActive =>
            Status == SaasTaskStatus.Queued || Status == SaasTaskStatus.Running;

        public virtual string StatusDescription
        {
            get
            {
                var description = $"{Name} {Status.ToString().ToLower()} ({Progress}%)";
                if (!StatusDetail.IsNullOrWhiteSpace()) description = $"{description} - {StatusDetail}";

                return description;
            }
        }
    }

    public enum SaasTaskStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
        TimedOut
    }
}