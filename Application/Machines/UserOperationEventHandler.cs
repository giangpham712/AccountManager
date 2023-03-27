using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.RecreateMachine;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateIdleSchedule;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.Logging;
using AccountManager.Application.Machines.Commands;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines
{
    public class UserOperationEventHandler :
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<MachineActionTriggeredEvent>,
        INotificationHandler<MachineUpdatedEvent>,
        INotificationHandler<MachineRecreatedEvent>,
        INotificationHandler<AccountPropertiesPushedEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>,
        INotificationHandler<BackupSettingsPushedEvent>,
        INotificationHandler<LicenseSettingsPushedEvent>,
        INotificationHandler<IdleSchedulePushedEvent>

    {
        private readonly ICloudStateDbContext _context;
        private readonly ILogger _logger;

        public UserOperationEventHandler(ILogger logger, ICloudStateDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(UserOperationTypes.CreateAccount, notification.Account.Machines.First().Id,
                notification.User, null, null, cancellationToken);
        }

        public async Task Handle(AccountPropertiesPushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines)
                await AddUserOperation(UserOperationTypes.PushAccountProperties, machine.Id, notification.User, null,
                    null, cancellationToken);
        }

        public async Task Handle(BackupSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines)
                await AddUserOperation(UserOperationTypes.PushBackupSettings, machine.Id, notification.User, null, null,
                    cancellationToken);
        }

        public async Task Handle(IdleSchedulePushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines)
                await AddUserOperation(UserOperationTypes.PushIdleSchedule, machine.Id, notification.User, null, null,
                    cancellationToken);
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines)
            {
                var machineChanges = notification.Changes.Where(x =>
                    x.EntityType == typeof(State).Name && x.EntityId == machine.DesiredState?.Id);

                var outputBuilder = new StringBuilder();

                outputBuilder.AppendLine("Machine state");
                outputBuilder.AppendLine();

                foreach (var change in machineChanges)
                    outputBuilder.AppendLine($"{change.PropertyName}: {change.OldValue} to {change.NewValue}");

                await AddUserOperation(UserOperationTypes.PushInstanceSettings, machine.Id, notification.User, null,
                    outputBuilder.ToString(), cancellationToken);
            }
        }

        public async Task Handle(LicenseSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var machine in notification.Account.Machines.Where(x => x.IsLauncher))
                await AddUserOperation(UserOperationTypes.PushLicenseSettings, machine.Id, notification.User, null,
                    null, cancellationToken);
        }

        public async Task Handle(MachineActionTriggeredEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(notification.Action, notification.Machine.Id, notification.User, notification.Params,
                null, cancellationToken);
        }


        public async Task Handle(MachineRecreatedEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(UserOperationTypes.RecreateMachine, notification.Machine.Id, notification.User,
                JsonConvert.SerializeObject(notification.Command), null, cancellationToken);
        }

        public async Task Handle(MachineUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await AddUserOperation(UserOperationTypes.UpdateMachine, notification.Machine.Id, notification.User,
                JsonConvert.SerializeObject(notification.Updates), null, cancellationToken);
        }

        private async Task AddUserOperation(string type, long machineId, string user, string operationParams,
            string output, CancellationToken cancellationToken)
        {
            _context.Set<UserOperation>().Add(new UserOperation
            {
                TypeName = type,
                MachineId = machineId,
                Timestamp = DateTimeOffset.Now,
                Params = operationParams,
                Output = output,
                User = user
            });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}