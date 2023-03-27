using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.UpdateAccount;
using AccountManager.Application.Accounts.Commands.UpdateBackupSettings;
using AccountManager.Application.Accounts.Commands.UpdateIdleSchedule;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Accounts.Commands.UpdateLicenseSettings;
using AccountManager.Application.Audit.Events;
using AccountManager.Application.Logging;
using AccountManager.Domain.Entities.Audit;
using AccountManager.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Audit
{
    public class AuditLogEventHandler :
        INotificationHandler<UserActionNotification>,
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<AccountPropertiesPushedEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>,
        INotificationHandler<BackupSettingsPushedEvent>,
        INotificationHandler<IdleSchedulePushedEvent>,
        INotificationHandler<LicenseSettingsPushedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuditLogEventHandler(ILogger logger, IAuditLogRepository auditLogRepository, IMapper mapper)
        {
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _mapper = mapper;
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("CreateAccount", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null, new { Params = notification.Command });
        }

        public async Task Handle(AccountPropertiesPushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushAccountProperties", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null,
                new { Params = notification.Command, notification.Changes });
        }

        public async Task Handle(BackupSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushBackupSettings", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null,
                new { Params = notification.Command, notification.Changes });
        }

        public async Task Handle(IdleSchedulePushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushIdleSchedule", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null,
                new { Params = notification.Command, notification.Changes });
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushInstanceSettings", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null,
                new { Params = notification.Command, notification.Changes });
        }

        public async Task Handle(LicenseSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            await AddAuditLogAsync("PushLicenseSettings", notification.User, DateTime.Now,
                new[] { _mapper.Map<AccountRef>(notification.Account) }, null,
                new { Params = notification.Command, notification.Changes });
        }

        public async Task Handle(UserActionNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Action = notification.Action,
                    User = notification.User,
                    Time = DateTime.UtcNow,
                    Accounts = _mapper.Map<AccountRef[]>(notification.Accounts),
                    Machines = _mapper.Map<MachineRef[]>(notification.Machines),
                    MetaData = notification.Data
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        private async Task AddAuditLogAsync(string action, string user, DateTime time, AccountRef[] accounts,
            MachineRef[] machines, dynamic metadata)
        {
            try
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Action = action,
                    User = user,
                    Time = time,
                    Accounts = accounts,
                    Machines = machines,
                    MetaData = metadata
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}