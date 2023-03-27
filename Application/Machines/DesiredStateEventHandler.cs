using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Commands.CreateAccount;
using AccountManager.Application.Accounts.Commands.RecreateMachine;
using AccountManager.Application.Accounts.Commands.UpdateInstanceSettings;
using AccountManager.Application.Logging;
using AccountManager.Application.Saas.Commands.CreateSite;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Machines
{
    public class DesiredStateEventHandler :
        INotificationHandler<AccountCreatedEvent>,
        INotificationHandler<InstanceSettingsPushedEvent>,
        INotificationHandler<SoftwareUpdatedEvent>,
        INotificationHandler<SiteCreatedEvent>,
        INotificationHandler<MachineRecreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;
        private readonly ILogger _logger;

        public DesiredStateEventHandler(ICloudStateDbContext context, ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Should no longer save historical desired states into this table
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(InstanceSettingsPushedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Should no longer save historical desired states into this table
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(MachineRecreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Should no longer save historical desired states into this table
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(SiteCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Should no longer save historical desired states into this table
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Handle(SoftwareUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                // Should no longer save historical desired states into this table
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}