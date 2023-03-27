using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AccountManager.Application.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManager.Application.Tasks
{
    public class TaskManager : ITaskManager
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly IDictionary<long, ActionBlock<ITask>> _machineTaskExecutors;
        private readonly BufferBlock<ITask> _taskForwarder;
        private readonly IHubContext<TaskHub, ITaskClient> _taskHub;
        private readonly List<ITask> _tasks = new List<ITask>();

        public TaskManager(IMapper mapper, IServiceScopeFactory serviceScopeFactory, IHubContext<TaskHub, ITaskClient> taskHub)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _taskHub = taskHub;
            _taskForwarder = new BufferBlock<ITask>();
            _machineTaskExecutors = new ConcurrentDictionary<long, ActionBlock<ITask>>();
        }

        public void Init()
        {
        }

        public async Task QueueTaskAsync(ITask task)
        {
            var taskExecutor = GetTaskExecutor(task.MachineId.Value);
            task.Id = Guid.NewGuid();
            task.QueuedAt = DateTimeOffset.Now;
            _tasks.Add(task);
            await _taskHub.Clients.All.TaskQueued(_mapper.Map<AMTaskDto>(task));
            var posted = taskExecutor.Post(task);
        }

        public void DeleteTask(ITask task)
        {
        }

        public List<ITask> ListTasksByMachine(long machineId)
        {
            return _tasks.Where(task => task.MachineId == machineId).ToList();
        }

        private ActionBlock<ITask> GetTaskExecutor(long groupId)
        {
            var taskExecutor = _machineTaskExecutors.TryGetValue(groupId, out var executor) ? executor : null;

            if (taskExecutor != null)
                return taskExecutor;

            taskExecutor = new ActionBlock<ITask>(async task =>
            {
                task.Status = TaskStatus.Running;
                task.StartedAt = DateTimeOffset.Now;

                try
                {
                    await _taskHub.Clients.All.TaskStarted(_mapper.Map<AMTaskDto>(task));

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        await task.ExecuteAsync(scope.ServiceProvider);
                    }

                    task.Status = TaskStatus.Completed;
                }
                catch (Exception e)
                {
                    task.Status = TaskStatus.Failed;
                    task.Error = e.Message;
                }
                finally
                {
                    task.FinishedAt = DateTimeOffset.Now;
                    await _taskHub.Clients.All.TaskFinished(_mapper.Map<AMTaskDto>(task));
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1
            });

            _machineTaskExecutors.Add(groupId, taskExecutor);

            return taskExecutor;
        }
    }
}